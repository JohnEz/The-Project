using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Engulf : BaseAbility {

	int baseDamage = 3;

	public Engulf(List<EventAction> _eventActions) : base (_eventActions) {
		range = 5;
		icon = "abilityEngulfController";
	}

	public override void UseAbility (UnitController caster, Node target)
	{
		base.UseAbility (caster, target);
		AddAbilityTarget (caster, target.myUnit, () => {
			caster.DealDamageTo(target.myUnit, baseDamage);
			target.myUnit.ApplyBuff(new Burn(3, caster.myStats.Power));
		});
	}

}
