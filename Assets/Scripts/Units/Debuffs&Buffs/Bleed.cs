using UnityEngine;
using System.Collections;

public class Bleed : Buff {

	public Bleed(int maxDuration, int power) : base (maxDuration) {
		name = "Bleed";
		maxStack = 5;
		isDebuff = true;
		flatMod [(int)Stats.DAMAGE] = (int)(power / 3);
		icon = "buffBleed";
	}

}
