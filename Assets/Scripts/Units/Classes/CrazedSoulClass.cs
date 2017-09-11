using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrazedSoulClass : UnitClass {

	public AudioClip abilitySound0;

	// Use this for initialization
	void Start () {
		List<EventAction> dualSlashActions = new List<EventAction> ();
		dualSlashActions.Add(EventAction.CreateAudioEventAction(Event.CAST_START, abilitySound0, EventTarget.CASTER));
		abilities [0] = new DualSlash (dualSlashActions);
	}

}
