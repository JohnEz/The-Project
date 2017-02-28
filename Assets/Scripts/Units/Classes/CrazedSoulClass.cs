using UnityEngine;
using System.Collections;

public class CrazedSoulClass : UnitClass {

	public AudioClip abilitySound0;

	// Use this for initialization
	void Start () {
		abilities [0] = new DualSlash (abilitySound0);
	}

}
