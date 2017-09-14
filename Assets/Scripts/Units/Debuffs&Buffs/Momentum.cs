using UnityEngine;
using System.Collections;

public class Momentum : Buff {

	public Momentum(int maxDuration) : base (maxDuration) {
		name = "Momentum";
		icon = "buffMomentum";
		flatMod [(int)Stats.SPEED] = 1;
		maxStack = 5;
	}

}
