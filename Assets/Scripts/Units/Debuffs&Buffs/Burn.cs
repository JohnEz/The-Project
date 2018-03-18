﻿using UnityEngine;
using System.Collections;

public class Burn : Buff {

	public Burn(int maxDuration, int power) : base (maxDuration) {
		name = "Burn";
		maxStack = 5;
		isDebuff = true;
		flatMod [(int)Stats.DAMAGE] = (int)(power / 4);
		icon = "buffBurn";
	}

}
