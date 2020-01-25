﻿using System.Collections.Generic;
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

    private void AbilityEffectUnit(UnitController caster, UnitController target) {
        // todo this should be effected by varing stats and weapon types
        float hitRoll = Random.Range(0, 20) + caster.myStats.Agility;

        if (hitRoll < target.myStats.AC) {
            target.CreateBasicText("Miss");
            return;
        }

        AddAbilityTarget(target.myNode, () => {
            attackEffects.ForEach(attackEffect => {
                attackEffect.AbilityEffect(caster, target);
            });
        });
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
            AbilityEffectUnit(caster, target.myUnit);
        } else {
            AbilityEffectNode(caster, target);
        }
        AbilityUsed();
    }

    // AOE on use
    public virtual void UseAbility(List<Node> effectedNodes, Node target) {
        OnUseAOEEventActions(effectedNodes, target);
        AbilityEffectNode(caster, target);
        effectedNodes.ForEach(node => {
            if (CanHitUnit(node)) {
                AbilityEffectUnit(caster, node.myUnit);
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
}