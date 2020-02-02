using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/New Attack Action")]
public class AttackAction : AbilityAction {
    public int range = 1;
    public int minRange = 0;

    public AreaOfEffect areaOfEffect = AreaOfEffect.SINGLE;
    public int aoeRange = 1;

    public TargetType targets = TargetType.ENEMY;
    public TileTarget tileTarget = TileTarget.UNIT;

    public bool canTargetSelf = false;
    public bool isAutoCast = false;

    public Stats targetStat = Stats.AC;
    public Stats attackStat = Stats.STRENGTH;

    public List<AttackEffect> attackEffects = new List<AttackEffect>(0);

    public bool CanTargetSelf {
        get { return canTargetSelf; }
        set { canTargetSelf = value; }
    }

    private void AbilityEffectUnit(UnitController caster, Node target) {
        UnitController targetUnit = target.MyUnit;
        bool isAllyAttack = caster.myPlayer.faction == target.MyUnit.myPlayer.faction;

        if (targetUnit.IsParrying() && !isAllyAttack && targetStat == Stats.AC) {
            targetUnit.Parry(caster);
            return;
        }

        bool hitTarget = isAllyAttack || CheckIfHit(caster, targetUnit);

        if (hitTarget) {
            AddAbilityTarget(target, () => {
                attackEffects.ForEach(attackEffect => {
                    attackEffect.AbilityEffect(caster, target);
                });
            });
        }
    }

    private bool CheckIfHit(UnitController caster, UnitController targetUnit) {
        float hitRoll = Random.Range(0, 20) + caster.myStats.GetStat(attackStat);

        int targetStatValue = targetUnit.myStats.GetStat(targetStat);
        int targetRoll = targetStat == Stats.AC ? targetStatValue : targetStatValue + 8;

        if (hitRoll < targetRoll) {
            targetUnit.CreateBasicText(targetStat == Stats.AC ? "Miss" : "Resist");
            return false;
        }

        return true;
    }

    private void AbilityEffectNode(UnitController caster, Node targetNode) {
        AddAbilityTarget(targetNode, () => {
            attackEffects.ForEach(attackEffect => {
                attackEffect.AbilityEffect(caster, targetNode);
            });
        });
    }

    // Single target on use
    public virtual void UseAbility(UnitController caster, Node target) {
        OnUseSingleEventActions(target);
        if (tileTarget == TileTarget.UNIT) {
            AbilityEffectUnit(caster, target);
        } else {
            AbilityEffectNode(caster, target);
        }
        AbilityUsed();
    }

    // AOE on use
    public virtual void UseAbility(List<Node> effectedNodes, Node target) {
        OnUseAOEEventActions(effectedNodes, target);
        //AbilityEffectNode(caster, target);
        effectedNodes.ForEach(node => {
            if (CanHitUnit(node.MyUnit)) {
                AbilityEffectUnit(caster, node);
            }
        });
        AbilityUsed();
    }

    public void AbilityUsed() {
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
                        if (CanHitUnit(targetNode.MyUnit)) {
                            eventAction.action(caster, targetNode);
                        }
                    });
                }
            }
        });
    }

    public bool CanTargetTile(Node targetNode) {
        if (tileTarget == TileTarget.UNIT && !CanHitUnit(targetNode.MyUnit)) {
            return false;
        }

        if (tileTarget == TileTarget.EMPTY_TILE && targetNode.MyUnit != null) {
            return false;
        }

        return true;
    }

    public bool CanHitUnit(UnitController targetUnit) {
        if (caster == targetUnit && !CanTargetSelf) {
            return false;
        }

        if (targetUnit && targetUnit.IsStealthed()) {
            return false;
        }

        bool hasTarget = targetUnit != null;
        bool isEnemy = hasTarget && targetUnit.myPlayer.faction != caster.myPlayer.faction;

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
}