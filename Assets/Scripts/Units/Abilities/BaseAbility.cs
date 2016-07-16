using UnityEngine;
using System.Collections;

public enum TargetType {
	ENEMY,
	ALLY,
	UNIT, //both enemies and allies
	TILE
}

public class BaseAbility {

	public int maxCooldown = 1;
	public int cooldown;

	public int range = 1;
	public int minRange = 1;

	public TargetType targets = TargetType.ENEMY;

	public virtual void UseAbility(UnitController caster, Node Target, Vector2 direction) {

	}

	public bool CanTargetTile(UnitController caster, Node targetNode) {
		bool canTarget = true;

		if (targets == TargetType.ENEMY & (targetNode.myUnit == null || targetNode.myUnit.myTeam == caster.myTeam)) {
			canTarget = false;
		}

		if (targets == TargetType.ALLY & (targetNode.myUnit == null || targetNode.myUnit.myTeam != caster.myTeam)) {
			canTarget = false;
		}

		if (targets == TargetType.UNIT & targetNode.myUnit == null) {
			canTarget = false;
		}

		return canTarget;
	}

	public int Cooldown {
		get { return cooldown; }
		set { cooldown = value; }
	}

	public int MaxCooldown {
		get { return maxCooldown; }
		set { maxCooldown = value; }
	}

}
