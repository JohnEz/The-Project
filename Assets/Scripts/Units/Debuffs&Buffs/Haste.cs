using UnityEngine;
using System.Collections;

public class Haste : Buff {

	public Haste(int maxDuration) : base (maxDuration) {
		flatMod [(int)Stats.AP] = 1;
	}

}
