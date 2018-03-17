using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Engulf : BaseAbility {

	float damageMod = 0.75f;

	public Engulf(List<EventAction> _eventActions) : base (_eventActions) {
		range = 5;
		icon = "abilityEngulfController";
	}

	public override void UseAbility (UnitController caster, Node target)
	{
		base.UseAbility (caster, target);
		AddAbilityTarget (caster, target.myUnit, () => {
			bool targetStillAlive = caster.DealDamageTo(target.myUnit, damageMod);
			if (targetStillAlive) {
				target.myUnit.ApplyBuff(new Burn(3, caster.myStats.Power));
			}
		});
	}

}
