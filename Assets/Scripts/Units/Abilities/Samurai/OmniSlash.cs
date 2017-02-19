using UnityEngine;
using System.Collections;

public class OmniSlash : BaseAbility {

	int baseDamage = 100;

	public override void UseAbility (UnitController caster, Node target, Vector2 direction)
	{
		base.UseAbility (caster, target, direction);
		AddDamageTarget (caster, target.myUnit, baseDamage);
	}

}
