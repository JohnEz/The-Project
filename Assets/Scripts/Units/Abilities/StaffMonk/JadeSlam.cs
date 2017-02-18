using UnityEngine;
using System.Collections;

public class JadeSlam : BaseAbility {

	int baseDamage = 7;

	public override void UseAbility (UnitController caster, Node target, Vector2 direction)
	{
		base.UseAbility (caster, target, direction);
		AddDamageTarget (caster, target.myUnit, baseDamage);
	}

}
