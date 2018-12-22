using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    private void AbilityEffectUnit(UnitController target) {
        attackEffects.ForEach(attackEffect => {
            AddAbilityTarget(target.myNode, () => {
                attackEffect.AbilityEffect(caster, target);
            });
        });
    }

    private void AbilityEffectNode(Node targetNode) {
        attackEffects.ForEach(attackEffect => {
            AddAbilityTarget(targetNode, () => {
                attackEffect.AbilityEffect(caster, targetNode);
            });
        });
    }

    // Single target on use
    public virtual void UseAbility(Node target) {
        OnUseSingleEventActions(target);
        AbilityEffectUnit(target.myUnit);
        AbilityEffectNode(target);
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
    }

    private void OnUseSingleEventActions(Node target) {
        eventActions.ForEach((eventAction) => {
            if (eventAction.eventTrigger == AbilityEvent.CAST_START) {
                eventAction.action(caster, target.myUnit, target);
            }
        });
    }

    private void OnUseAOEEventActions(List<Node> effectedNodes, Node target) {
        eventActions.ForEach((eventAction) => {
            if (eventAction.eventTrigger == AbilityEvent.CAST_START) {
                if (eventAction.eventTarget == EventTarget.CASTER || eventAction.eventTarget == EventTarget.TARGETEDTILE) {

                    eventAction.action(caster, null, target);

                } else if (eventAction.eventTarget == EventTarget.TARGETUNIT) {

                    effectedNodes.ForEach((targetNode) => {
                        if (CanHitUnit(targetNode)) {
                            eventAction.action(caster, targetNode.myUnit, target);
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

    public bool CanTargetSelf {
        get { return canTargetSelf; }
        set { canTargetSelf = value; }
    }
}
