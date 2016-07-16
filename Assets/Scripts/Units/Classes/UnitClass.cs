using UnityEngine;
using System.Collections;

public class UnitClass : MonoBehaviour {

	public BaseAbility[] abilities = new BaseAbility[5];



	public bool CanUseAbility(int abil) {
		if (abil >= 0 && abil < abilities.Length && abilities [abil] != null) {
			return true;
		}

		return false;
	}
}
