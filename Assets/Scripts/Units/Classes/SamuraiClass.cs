using UnityEngine;
using System.Collections;

public class SamuraiClass : UnitClass {

	public AudioClip abilitySound0;

	// Use this for initialization
	void Start () {
		abilities [0] = new OmniSlash (abilitySound0);
	}

}
