using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Card/New Attack Action")]
public class AttackAction : CardAction {
    public int range = 1;
    public int minRange = 1;

    public AreaOfEffect areaOfEffect = AreaOfEffect.SINGLE;
    public int aoeRange = 1;

    public TargetType targets = TargetType.ENEMY;
    public TileTarget tileTarget = TileTarget.UNIT;

    public bool canTargetSelf = false;

    public List<AttackEffect> attackEffects = new List<AttackEffect>(0);

    // AI Variables
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
        Debug.Log("Checking if " + name + " is on cooldown: " + cooldown);
        return Cooldown > 0;
    }

    private void AbilityEffectUnit(UnitController target) {
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
        Debug.Log("Setting cooldown " + MaxCooldown + " for ability " + name);
        Cooldown = MaxCooldown;
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
        Cooldown = MaxCooldown;
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
                            eventAction.action(caster, target);
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

    public int GetDamageEstimate() {
        int damage = 0;

        attackEffects.ForEach(attackEffect => {
            if (attackEffect.GetType() == typeof(DamageEffect)) {
                damage += ((DamageEffect)attackEffect).damage;
            } else if (attackEffect.GetType() == typeof(DamagePerStackEffect)) {
                damage += ((DamagePerStackEffect)attackEffect).damageMod * 2;
            } else if (attackEffect.GetType() == typeof(DamageWithMultiplierEffect)) {
                DamageWithMultiplierEffect damageEffect = (DamageWithMultiplierEffect)attackEffect;
                damage += damageEffect.baseDamage * (damageEffect.damageMod / 2);
            }
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

    public int GetArmourEstimate() {
        int armour = 0;

        attackEffects.ForEach(attackEffect => {
            if (attackEffect.GetType() == typeof(IncreaseArmour)) {
                armour += ((IncreaseArmour) attackEffect).armourIncrease;
            }
        });

        return armour;
    }
}