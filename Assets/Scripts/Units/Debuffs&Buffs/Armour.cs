using UnityEngine;
using System.Collections;

public class Armour : Buff {

	public Armour(int maxDuration) : base (maxDuration) {
		name = "Armour";
		icon = "buffArmour";
		maxStack = 5;
		flatMod [(int)Stats.ARMOUR] = 1;
	}

}
