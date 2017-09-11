using UnityEngine;
using System.Collections;

public class Vitalise : Buff {

	public Vitalise(int maxDuration, int power) : base (maxDuration) {
		maxStack = 5;
		flatMod [(int)Stats.HEALTH] = 5 + (int)(power / 2);
	}

}
