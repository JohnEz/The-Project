using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StaffMonkClass : UnitClass {

	// Use this for initialization
	void Start () {
		abilities [0] = new JadeSlam (new List<EventAction>());
	}

}
