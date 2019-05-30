using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Card/New Attack Action")]
public class AttackAction : AbilityAction {
    public int range = 1;
    public int minRange = 0;

    public AreaOfEffect areaOfEffect = AreaOfEffect.SINGLE;
    public int aoeRange = 1;

    public TargetType targets = TargetType.ENEMY;
    public TileTarget tileTarget = TileTarget.UNIT;

    public bool canTargetSelf = false;
    public bool isAutoCast = false;
    public bool canDodge = true;

    public List<AttackEffect> attackEffects = new List<AttackEffect>(0);

    // TODO remove for ability cooldown AI Variables
    private int cooldown;

    public int maxCooldown = 1;

    public bool CanTargetSelf {
        get { return canTargetSelf; }
        set { canTargetSelf = value; }
    }

    public int Cooldown {
        get { return cooldown; }
        set { cooldown = value; }
    }

    public int MaxCooldown {
        get { return maxCooldown; }
        set { maxCooldown = value; }
    }

    public bool IsOnCooldown() {
        return Cooldown > 0;
    }

    private void AbilityEffectUnit(UnitController target) {
        if (canDodge) {
            float hitChance = (float)caster.myStats.Accuracy / target.myStats.Dodge;
            float dodgeRoll = Random.value;
            if (hitChance < dodgeRoll) {
                target.CreateBasicText("Dodge");
                return;
            }
        }

        AddAbilityTarget(target.myNode, () => {
            attackEffects.ForEach(attackEffect => {
                attackEffect.AbilityEffect(caster, target);
            });
        });
    }

    private void AbilityEffectNode(Node targetNode) {
        AddAbilityTarget(targetNode, () => {
            attackEffects.ForEach(attackEffect => {
                attackEffect.AbilityEffect(caster, targetNode);
            });
        });
    }

    // Single target on use
    public virtual void UseAbility(Node target) {
        OnUseSingleEventActions(target);
        if (tileTarget == TileTarget.UNIT) {
            AbilityEffectUnit(target.myUnit);
        } else {
            AbilityEffectNode(target);
        }
        AbilityUsed();
    }

    // AOE on use
    public virtual void UseAbility(List<Node> effectedNodes, Node target) {
        OnUseAOEEventActions(effectedNodes, target);
        AbilityEffectNode(target);
        effectedNodes.ForEach(node => {
            if (CanHitUnit(node)) {
                AbilityEffectUnit(node.myUnit);
            }
        });
        AbilityUsed();
    }

    public void AbilityUsed() {
        Cooldown = MaxCooldown;

        if (caster.IsStealthed()) {
            Buff stealthBuff = caster.myStats.buffs.FindBuff("Stealth");
            caster.myStats.RemoveBuff(stealthBuff);
        }
    }

    // Creates effects on cast start
    private void OnUseSingleEventActions(Node target) {
        eventActions.ForEach((eventAction) => {
            if (eventAction.eventTrigger == AbilityEvent.CAST_START) {
                eventAction.action(caster, target);
            }
        });
    }

    // Creates effects on cast start
    private void OnUseAOEEventActions(List<Node> effectedNodes, Node target) {
        eventActions.ForEach((eventAction) => {
            if (eventAction.eventTrigger == AbilityEvent.CAST_START) {
                if (eventAction.eventTarget == EventTarget.CASTER || eventAction.eventTarget == EventTarget.TARGETEDTILE) {
                    eventAction.action(caster, target);
                } else if (eventAction.eventTarget == EventTarget.TARGETUNIT) {
                    effectedNodes.ForEach((targetNode) => {
                        if (CanHitUnit(targetNode)) {
                            eventAction.action(caster, targetNode);
                        }
                    });
                }
            }
        });
    }

    public bool CanTargetTile(Node targetNode) {
        if (tileTarget == TileTarget.UNIT && !CanHitUnit(targetNode)) {
            return false;
        }

        if (tileTarget == TileTarget.EMPTY_TILE && targetNode.myUnit != null) {
            return false;
        }

        return true;
    }

    public bool CanHitUnit(Node targetNode) {
        if (caster.myNode == targetNode && !CanTargetSelf) {
            return false;
        }

        if (targetNode.myUnit && targetNode.myUnit.IsStealthed()) {
            return false;
        }

        bool hasTarget = targetNode.myUnit != null;
        bool isEnemy = hasTarget && targetNode.myUnit.myPlayer.faction != caster.myPlayer.faction;

        switch (targets) {
            case TargetType.ENEMY:
                return isEnemy;

            case TargetType.ALLY:
                return hasTarget && !isEnemy;

            case TargetType.UNIT:
                return hasTarget;

            default:
                return false;
        }
    }

    public override int GetDamage(UnitController caster) {
        int damage = 0;

        attackEffects.ForEach(attackEffect => {
            //if (attackEffect.GetType() == typeof(DamageEffect)) {
            //    damage += ((DamageEffect)attackEffect).damage;
            //} else if (attackEffect.GetType() == typeof(PowerDamageEffect)) {
            //    PowerDamageEffect damageEffect = (PowerDamageEffect)attackEffect;
            //    damage += damageEffect.PowerModToInt(power, damageEffect.powerMod);
            //}
            damage += attackEffect.GetDamage(caster);
        });

        return damage;
    }

    public override int GetHealing(UnitController caster) {
        int healing = 0;

        attackEffects.ForEach(attackEffect => {
            //if (attackEffect.GetType() == typeof(HealEffect)) {
            //    healing += ((HealEffect)attackEffect).healing;
            //} else if (attackEffect.GetType() == typeof(PowerHealEffect)) {
            //    PowerHealEffect healEffect = (PowerHealEffect)attackEffect;
            //    healing += healEffect.PowerModToInt(power, healEffect.powerMod);
            //}
            healing += attackEffect.GetHealing(caster);
        });

        return healing;
    }

    public override int GetShield(UnitController caster) {
        int shield = 0;

        attackEffects.ForEach(attackEffect => {
            //if (attackEffect.GetType() == typeof(ShieldEffect)) {
            //    ShieldEffect healEffect = (ShieldEffect)attackEffect;
            //    shield += healEffect.PowerModToInt(power, healEffect.powerMod);
            //}
            shield += attackEffect.GetShield(caster);
        });

        return shield;
    }

    // AI Estimates //
    //////////////////

    public int GetDamageEstimate() {
        int damage = 0;

        attackEffects.ForEach(attackEffect => {
            if (attackEffect.GetType() == typeof(DamageEffect)) {
                damage += ((DamageEffect)attackEffect).damage;
            }
            //    } else if (attackEffect.GetType() == typeof(DamagePerStackEffect)) {
            //        damage += ((DamagePerStackEffect)attackEffect).damageMod * 2;
            //    } else if (attackEffect.GetType() == typeof(DamageWithMultiplierEffect)) {
            //        DamageWithMultiplierEffect damageEffect = (DamageWithMultiplierEffect)attackEffect;
            //        damage += damageEffect.baseDamage * (damageEffect.damageMod / 2);
            //    }
        });

        return damage;
    }

    public int GetHealingEstimate() {
        int healing = 0;

        attackEffects.ForEach(attackEffect => {
            if (attackEffect.GetType() == typeof(HealEffect)) {
                healing += ((HealEffect)attackEffect).healing;
            }
        });

        return healing;
    }

    public bool AppliesStealth() {
        return attackEffects.Exists(attackEffect => attackEffect.GetType() == typeof(StealthEffect));
    }
}