using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DemoralisingShout : BaseAbility {

    int buffDuration = 2;

    public DemoralisingShout(List<EventAction> _eventActions, UnitController caster) : base(_eventActions, caster) {
        areaOfEffect = AreaOfEffect.AURA;
        tileTarget = TileTarget.TILE;
        maxCooldown = 3;
        aoeRange = 3;
        icon = "abilityRallyController";
        Name = "Demoralising Shout";
    }

    public override void UseAbility(Node target) {
        if (CanHitUnit(target)) {
            caster.AddAbilityTarget(target.myUnit, () => {
                target.myUnit.ApplyBuff(new Weaken(buffDuration));
                target.myUnit.ApplyBuff(new Slow(buffDuration));
            });
        }
    }

    public override void UseAbility(List<Node> targets, Node targetedNode) {
        base.UseAbility(targets, targetedNode);
        targets.ForEach(target => UseAbility(target));
    }

    public override string GetDescription() {
        return base.GetDescription() + "Demoralises enemies around him, applying Weaken and Slow for " + buffDuration + " turns.";
    }
}