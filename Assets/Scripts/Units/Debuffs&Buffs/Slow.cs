using UnityEngine;
using System.Collections;

public class Slow : Buff {

	public Slow(int maxDuration) : base (maxDuration) {
		name = "Slow";
		icon = "buffMomentum";
		flatMod [(int)Stats.SPEED] = -1;
		maxStack = 5;
		isDebuff = true;
	}

}
