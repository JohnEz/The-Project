using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitClass : MonoBehaviour {

	public List<BaseAbility> abilities = new List<BaseAbility> ();
	[SerializeField]
	public AudioClip onHitSfx;
	[SerializeField]
	public AudioClip onDeathSfx;

	public virtual void Initialise() {

	}

	public bool CanUseAbility(int abil) {
		if (abil >= 0 && abil < abilities.Count) {
			return true;
		}

		return false;
	}
}
