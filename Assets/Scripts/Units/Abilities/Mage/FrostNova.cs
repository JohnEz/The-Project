using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FrostNova : BaseAbility {

	float damageMod = 0.33f;
	int duration = 2;

	GameObject persistentSnareFxPrefab;

	public FrostNova(List<EventAction> _eventActions, GameObject snareFx) : base (_eventActions) {
		range = 5;
		areaOfEffect = AreaOfEffect.CIRCLE;
		aoeRange = 2;
		maxCooldown = 3;
		tileTarget = TileTarget.TILE;
		persistentSnareFxPrefab = snareFx;
		icon = "abilityFrostNovaController";
		Name = "Frost Nova";
		Description = "Range: " + range + "\nFreezes ememies in an area, dealing " + damageMod + "x power damage and snaring them for " + duration + " turns.";
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

}
