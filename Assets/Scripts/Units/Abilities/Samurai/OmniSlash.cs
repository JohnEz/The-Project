using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OmniSlash : BaseAbility {

	float damageMod = 1.25f;

	public OmniSlash(List<EventAction> _eventActions, UnitController caster) : base (_eventActions, caster) {
		Name = "Omni Slash";
	}

	public override void UseAbility (Node target)
	{
		base.UseAbility (target);
		AddAbilityTarget (target.myUnit, () => {
			caster.DealDamageTo(target.myUnit, damageMod);
			caster.ApplyBuff(new Armour(2));
		});
	}

	public override string GetDescription() {
		return base.GetDescription() + "Slash an enemy for " + (int)(damageMod * caster.myStats.Power) + " damage and gives the Samurai 1 stack of armour.";
	}

}
