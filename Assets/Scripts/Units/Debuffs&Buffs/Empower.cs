using UnityEngine;
using System.Collections;

public class Empower : Buff {

	public Empower(int duration) : base () {
		maxStack = 5;
		percentMod [(int)Stats.POWER] = 1.2f;
		maxDuration = duration;
	}

}
