using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SamuraiClass : UnitClass {

	public AudioClip abilitySound0;

	// Use this for initialization
	public override void Initialise(UnitStats casterStats) {
		List<EventAction> omniSlashActions = new List<EventAction> ();
		omniSlashActions.Add(EventAction.CreateAudioEventAction(Event.CAST_START, abilitySound0, EventTarget.CASTER));
		abilities.Add(new OmniSlash (omniSlashActions, casterStats));

		List<EventAction> cleaveActions = new List<EventAction> ();
		omniSlashActions.Add(EventAction.CreateAudioEventAction(Event.CAST_START, abilitySound0, EventTarget.CASTER));
		abilities.Add(new Cleave (cleaveActions, casterStats));
	}

}
