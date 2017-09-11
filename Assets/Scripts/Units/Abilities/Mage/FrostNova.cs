using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FrostNova : BaseAbility {

	int baseDamage = 3;

	GameObject persistentSnareFxPrefab;

	public FrostNova(List<EventAction> _eventActions, GameObject snareFx) : base (_eventActions) {
		range = 5;
		areaOfEffect = AreaOfEffect.CIRCLE;
		aoeRange = 2;
		tileTarget = TileTarget.TILE;
		persistentSnareFxPrefab = snareFx;
	}

	public override void UseAbility (UnitController caster, Node target)
	{
		if (CanHitUnit(caster, target)) {
			AddAbilityTarget (caster, target.myUnit, () => {
				bool targetStillAlive = caster.DealDamageTo(target.myUnit, baseDamage);
				if (targetStillAlive) {
					target.myUnit.ApplyBuff (new Snare (2, persistentSnareFxPrefab));
				}
			});
		}
	}

	public override void UseAbility(UnitController caster, List<Node> targets, Node targetedNode) {
		base.UseAbility (caster, targets, targetedNode);
		targets.ForEach (target => UseAbility (caster, target));
	}

}
