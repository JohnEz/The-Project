using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Combustion : BaseAbility {

	float damageMod = 0.6f;
	int duration = 3;

	public Combustion(List<EventAction> _eventActions, UnitController caster) : base (_eventActions, caster) {
		range = 6;
		icon = "abilityEngulfController";
		Name = "Flaming Arrow";
	}

	public override void UseAbility (Node target)
	{
		base.UseAbility (target);
		AddAbilityTarget (target.myUnit, () => {
			Buff burnBuff = target.myUnit.myStats.FindBuff("Burn");
			int burnCount = 0;

			if (burnBuff != null) {
				target.myUnit.myStats.RemoveBuff(burnBuff, true);
                burnCount = burnBuff.stacks;
			}

			caster.DealDamageTo(target.myUnit, damageMod * burnCount, true);

		});
	}

	public override string GetDescription() {
		return base.GetDescription() + "Removes all the burns on a target, ignoring armour, and dealing " + (int)(damageMod * caster.myStats.Power) + " damage times the number of burns";
	}

}
