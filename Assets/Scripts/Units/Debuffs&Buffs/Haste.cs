using UnityEngine;
using System.Collections;

public class Haste : Buff {

	public Haste(int duration) : base () {
		flatMod [(int)Stats.AP] = 1;
		maxDuration = duration;
	}

}
