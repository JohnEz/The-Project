using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OmniSlash : BaseAbility {

	float damageMod = 1.25f;

	public OmniSlash(List<EventAction> _eventActions) : base (_eventActions) {
		Name = "Omni Slash";
		Description = "Range: " + range + "\nSlash an enemy " + damageMod + "x power damage.";
	}

	public override void UseAbility (UnitController caster, Node target)
	{
		base.UseAbility (caster, target);
		AddAbilityTarget (caster, target.myUnit, () => {caster.DealDamageTo(target.myUnit, damageMod);});
	}

}
