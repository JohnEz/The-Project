﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JadeSlam : BaseAbility {

	int baseDamage = 7;

	public JadeSlam (List<EventAction> _eventActions) : base (_eventActions) {

	}

	public override void UseAbility (UnitController caster, Node target)
	{
		base.UseAbility (caster, target);
		AddAbilityTarget (caster, target.myUnit, () => {caster.DealDamageTo(target.myUnit, baseDamage);});
	}

}
