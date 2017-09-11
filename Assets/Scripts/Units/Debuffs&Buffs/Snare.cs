﻿using UnityEngine;
using System.Collections;

public class Snare : Buff {

	public Snare(int maxDuration, GameObject persistentFx) : base (maxDuration, persistentFx) {
		percentMod [(int)Stats.SPEED] = 0;
		isDebuff = true;
	}

}
