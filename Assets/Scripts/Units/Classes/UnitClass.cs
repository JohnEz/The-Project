using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitClass : MonoBehaviour {

	public List<BaseAbility> abilities = new List<BaseAbility> ();
	[SerializeField]
	public AudioClip onHitSfx;
	[SerializeField]
	public AudioClip onDeathSfx;

	public string className = "CLASSNAME";

	public UnitClass() {

	}

	public virtual void Initialise(UnitController caster) {

	}

	public void NewTurn () {
		foreach (BaseAbility ability in abilities) {
			ability.NewTurn ();
		}
	}

	public bool HasAbility(int abil) {
		return abil >= 0 && abil < abilities.Count;
	}
}
