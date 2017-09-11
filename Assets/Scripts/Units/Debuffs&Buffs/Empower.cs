using UnityEngine;
using System.Collections;

public class Empower : Buff {

	public Empower(int maxDuration) : base (maxDuration) {
		maxStack = 5;
		percentMod [(int)Stats.POWER] = 1.2f;
	}

}
