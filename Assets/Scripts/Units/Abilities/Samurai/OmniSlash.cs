using UnityEngine;
using System.Collections;

public class OmniSlash : BaseAbility {

	int baseDamage = 10;
	float armourGain = 0.1f;

	public override void UseAbility (UnitController caster, Node target, Vector2 direction)
	{
		base.UseAbility (caster, target, direction);
		bool hit = caster.DealDamageTo (target.myUnit, baseDamage);

		if (hit) {
			//TODO Caster should gain armourGain% of max hp as armour
		}

	}

}
