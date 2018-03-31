using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Combustion : BaseAbility {

	float damageMod = 0.5f;
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
			float burnDamage = 0;

			if (burnBuff != null) {
				target.myUnit.myStats.RemoveBuff(burnBuff, true);
				burnDamage = burnBuff.GetFlatMod((int)Stats.DAMAGE) * burnBuff.duration;
			}

			caster.DealDamageTo(target.myUnit, damageMod, true, burnDamage);

		});
	}

	public override string GetDescription() {
		return base.GetDescription() + "Ignites all the burns on a target dealing " + (int)(damageMod * caster.myStats.Power) + " damage plus the remaining damage of the burns.";
	}

}
