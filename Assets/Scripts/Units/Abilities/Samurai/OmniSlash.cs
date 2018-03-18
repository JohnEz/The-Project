using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OmniSlash : BaseAbility {

	float damageMod = 1.25f;

	public OmniSlash(List<EventAction> _eventActions, UnitStats casterStats) : base (_eventActions, casterStats) {
		Name = "Omni Slash";
	}

	public override void UseAbility (UnitController caster, Node target)
	{
		base.UseAbility (caster, target);
		AddAbilityTarget (caster, target.myUnit, () => {caster.DealDamageTo(target.myUnit, damageMod);});
	}

	public override string GetDescription() {
		return "Range: " + range + "\nSlash an enemy " + (int)(damageMod * casterStats.Power) + " damage.";
	}

}
