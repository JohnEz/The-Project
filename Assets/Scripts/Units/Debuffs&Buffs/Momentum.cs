using UnityEngine;
using System.Collections;

public class Momentum : Buff {

	public Momentum(int maxDuration) : base (maxDuration) {
		flatMod [(int)Stats.SPEED] = 1;
	}

}
