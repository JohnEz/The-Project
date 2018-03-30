using UnityEngine;
using System.Collections;

public class Snare : Buff {

	public Snare(int maxDuration, GameObject persistentFx = null) : base (maxDuration, persistentFx) {
		name = "Snare";
		percentMod [(int)Stats.SPEED] = 0;
		isDebuff = true;
		icon = "buffSnare";
	}

}
