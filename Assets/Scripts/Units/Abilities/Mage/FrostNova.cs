using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FrostNova : BaseAbility {

	float damageMod = 0.33f;
	int duration = 2;

	GameObject persistentSnareFxPrefab;

	public FrostNova(List<EventAction> _eventActions, GameObject snareFx, UnitStats casterStats) : base (_eventActions, casterStats) {
		range = 5;
		areaOfEffect = AreaOfEffect.CIRCLE;
		aoeRange = 2;
		maxCooldown = 3;
		tileTarget = TileTarget.TILE;
		persistentSnareFxPrefab = snareFx;
		icon = "abilityFrostNovaController";
		Name = "Frost Nova";
	}

	public override void UseAbility (UnitController caster, Node target)
	{
		if (CanHitUnit(caster, target)) {
			AddAbilityTarget (caster, target.myUnit, () => {
				bool targetStillAlive = caster.DealDamageTo(target.myUnit, damageMod);
				if (targetStillAlive) {
					target.myUnit.ApplyBuff (new Snare (duration, persistentSnareFxPrefab));
				}
			});
		}
	}

	public override void UseAbility(UnitController caster, List<Node> targets, Node targetedNode) {
		base.UseAbility (caster, targets, targetedNode);
		targets.ForEach (target => UseAbility (caster, target));
	}

	public override string GetDescription() {
		return base.GetDescription() + "Freezes ememies in an area, dealing " + (int)(damageMod * casterStats.Power) + " damage and snaring them for " + duration + " turns.";
	}

}
