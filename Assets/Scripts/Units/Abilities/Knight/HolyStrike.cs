using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HolyStrike : BaseAbility {

	float damageMod = 1;

	public HolyStrike(List<EventAction> _eventActions) : base (_eventActions) {
		icon = "abilityHolyStrikeController";
		Name = "Holy Strike";
		Description = "Range: " + range + "\nAttack with holy might dealing " + damageMod + "x power damage.";
	}

	public override void UseAbility (UnitController caster, Node target)
	{
		base.UseAbility (caster, target);
		AddAbilityTarget (caster, target.myUnit, () => {caster.DealDamageTo(target.myUnit, damageMod);});
	}

}
