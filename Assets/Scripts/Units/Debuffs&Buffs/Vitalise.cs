using UnityEngine;
using System.Collections;

public class Vitalise : Buff {

	public Vitalise(int duration, int power) : base () {
		maxStack = 5;
		flatMod [(int)Stats.HEALTH] = 5 + (int)(power / 2);
		maxDuration = duration;
	}

}
