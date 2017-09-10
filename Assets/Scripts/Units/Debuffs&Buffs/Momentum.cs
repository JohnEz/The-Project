using UnityEngine;
using System.Collections;

public class Momentum : Buff {

	public Momentum(int duration) : base () {
		flatMod [(int)Stats.SPEED] = 1;
		maxDuration = duration;
	}

}
