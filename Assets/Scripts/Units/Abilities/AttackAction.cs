using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Card/AttackAction")]
public class AttackAction : CardAction {

    public int range = 1;
    public int minRange = 1;

    public AreaOfEffect areaOfEffect = AreaOfEffect.SINGLE;
    public int aoeRange = 1;

    public TargetType targets = TargetType.ENEMY;
    public TileTarget tileTarget = TileTarget.UNIT;

    public bool canTargetSelf = false;

    // Single target on use
    public virtual void UseAbility(Node target) {
        OnUseSingleEventActions(target);
    }

    // AOE on use
    public virtual void UseAbility(List<Node> targets, Node target) {
        OnUseAOEEventActions(targets, target);
    }

    private void OnUseSingleEventActions(Node target) {
        eventActions.ForEach((eventAction) => {
            if (eventAction.eventTrigger == AbilityEvent.CAST_START) {
                eventAction.action(caster, target.myUnit, target);
            }
        });
    }

    private void OnUseAOEEventActions(List<Node> targets, Node target) {
        eventActions.ForEach((eventAction) => {
            if (eventAction.eventTrigger == AbilityEvent.CAST_START) {
                if (eventAction.eventTarget == EventTarget.CASTER || eventAction.eventTarget == EventTarget.TARGETEDTILE) {

                    eventAction.action(caster, null, target);

                } else if (eventAction.eventTarget == EventTarget.TARGETUNIT) {

                    targets.ForEach((targetNode) => {
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
