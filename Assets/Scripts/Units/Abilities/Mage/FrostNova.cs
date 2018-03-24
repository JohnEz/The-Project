using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FrostNova : BaseAbility {

	float damageMod = 0.33f;
	int duration = 2;

	GameObject persistentSnareFxPrefab;

	public FrostNova(List<EventAction> _eventActions, GameObject snareFx, UnitController caster) : base (_eventActions, caster) {
		range = 5;
		areaOfEffect = AreaOfEffect.CIRCLE;
		aoeRange = 2;
		maxCooldown = 3;
		tileTarget = TileTarget.TILE;
		persistentSnareFxPrefab = snareFx;
		icon = "abilityFrostNovaController";
		Name = "Frost Nova";
	}

	public override void UseAbility (Node target)
	{
		if (CanHitUnit(target)) {
			AddAbilityTarget (target.myUnit, () => {
				bool targetStillAlive = caster.DealDamageTo(target.myUnit, damageMod);
				if (targetStillAlive) {
					target.myUnit.ApplyBuff (new Snare (duration, persistentSnareFxPrefab));
				}
			});
		}
	}

	public override void UseAbility(List<Node> targets, Node targetedNode) {
		base.UseAbility (targets, targetedNode);
		targets.ForEach (target => UseAbility (target));
	}

	public override string GetDescription() {
		return base.GetDescription() + "Freezes ememies in an area, dealing " + (int)(damageMod * caster.myStats.Power) + " damage and snaring them for " + duration + " turns.";
	}

}
