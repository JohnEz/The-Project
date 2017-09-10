using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DualSlash : BaseAbility {

	int baseDamage = 10;

	public DualSlash(List<EventAction> _eventActions) : base (_eventActions) {

	}

	public override void UseAbility (UnitController caster, Node target)
	{
		base.UseAbility (caster, target);
		AddAbilityTarget (caster, target.myUnit, () => {caster.DealDamageTo(target.myUnit, baseDamage);});
	}

}
