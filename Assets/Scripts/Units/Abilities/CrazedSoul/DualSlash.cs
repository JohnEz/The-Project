using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DualSlash : BaseAbility {

	float damageMod = 1.25f;

	public DualSlash(List<EventAction> _eventActions, UnitStats casterStats) : base (_eventActions, casterStats) {
		Name = "Dual Slash";
	}

	public override void UseAbility (UnitController caster, Node target)
	{
		base.UseAbility (caster, target);
		AddAbilityTarget (caster, target.myUnit, () => {caster.DealDamageTo(target.myUnit, damageMod);});
	}

	public override string GetDescription() {
		return base.GetDescription() + "Attack using both blades dealing " + (int)(damageMod * casterStats.Power) + " damage.";
	}
}
