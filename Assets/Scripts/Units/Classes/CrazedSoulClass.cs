using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrazedSoulClass : UnitClass {

	public AudioClip abilitySound0;

	public CrazedSoulClass() {
		className = "Crazed Soul";
	}

	// Use this for initialization
	public override void Initialise(UnitStats casterStats) {
		List<EventAction> dualSlashActions = new List<EventAction> ();
		dualSlashActions.Add(EventAction.CreateAudioEventAction(Event.CAST_START, abilitySound0, EventTarget.CASTER));
		abilities.Add(new DualSlash (dualSlashActions, casterStats));
	}

}
