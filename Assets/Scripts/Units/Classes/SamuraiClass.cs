using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SamuraiClass : UnitClass {

	public AudioClip abilitySound0;

	// Use this for initialization
	void Start () {
		List<EventAction> omniSlashActions = new List<EventAction> ();
		omniSlashActions.Add(EventAction.CreateAudioEventAction(Event.CAST_START, abilitySound0, true));
		abilities [0] = new OmniSlash (omniSlashActions);
	}

}
