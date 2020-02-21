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

    public static bool AttackerHasAdvantage(UnitController attacker, UnitController defender) {
        bool hasAdvantage = attacker.HasAdvantageToAttack() || defender.HasAdvantageAgainstMe();
        bool hasDisadvantage = attacker.HasDisdvantageToAttack() || defender.HasDisadvantageAgainstMe();

        return hasAdvantage && !hasDisadvantage;
    }

    public static bool AttackerHasDisadvantage(UnitController attacker, UnitController defender) {
        bool hasAdvantage = attacker.HasAdvantageToAttack() || defender.HasAdvantageAgainstMe();
        bool hasDisadvantage = attacker.HasDisdvantageToAttack() || defender.HasDisadvantageAgainstMe();

        return hasDisadvantage && !hasAdvantage;
    }

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

        bool hasAdvantage = AttackerHasAdvantage(caster, targetUnit);
        bool hasDisadvantage = AttackerHasDisadvantage(caster, targetUnit);

        bool hitTarget = isAllyAttack || CheckIfHit(caster, targetUnit, hasAdvantage, hasDisadvantage);

        if (hitTarget) {
            AddAbilityTarget(target, () => {
                attackEffects.ForEach(attackEffect => {
                    attackEffect.AbilityEffect(caster, target);
                });
            });
        }
    }

    private bool CheckIfHit(UnitController caster, UnitController targetUnit, bool hasAdvantage, bool hasDisadvantage) {
        int hitRoll1 = Random.Range(0, 20);
        int hitRoll2 = Random.Range(0, 20);
        int finalRoll = hitRoll1;

        if (hasAdvantage && !hasDisadvantage) {
            finalRoll = Mathf.Max(hitRoll1, hitRoll2);
        } else if (hasDisadvantage && !hasAdvantage) {
            finalRoll = Mathf.Min(hitRoll1, hitRoll2);
        }

        Debug.Log(string.Format("Advantage: {0}, Disadvantage: {1}, Roll1: {2}, Roll2: {3}, Final Roll: {4}", hasAdvantage, hasDisadvantage, hitRoll1, hitRoll2, finalRoll));

        Tile blindSpot = AIManager.GetBlindSpot(targetUnit);
        int blindSpotMod = blindSpot && blindSpot.OverlapsTile(caster.myTile) ? 1 : 0;

        int attackValue = caster.myStats.GetStat(attackStat) + caster.myStats.Hit + blindSpotMod;

        int targetStatValue = targetUnit.myStats.GetStat(targetStat);
        int targetRoll = targetStat == Stats.AC ? targetStatValue : targetStatValue + 8;

        //Debug.Log(string.Format("Roll: {0}, BlindSpot: {1}, AttackValue: {2}, AC: {3}", hitRoll, blindSpotMod, attackValue, targetRoll));

        if (finalRoll + attackValue < targetRoll) {
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