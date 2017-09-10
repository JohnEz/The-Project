using UnityEngine;
using System.Collections;

public class Snare : Buff {

	public Snare(int duration) : base () {
		percentMod [(int)Stats.SPEED] = 0;
		maxDuration = duration;
		isDebuff = true;
	}

}
