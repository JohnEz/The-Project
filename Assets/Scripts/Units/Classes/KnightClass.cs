using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KnightClass : UnitClass {
	
	[SerializeField]
	AudioClip holyStrikeSFx;

	[SerializeField]
	GameObject rallyFx;
	[SerializeField]
	AudioClip rallySfx;

	// Use this for initialization
	void Start () {
		List<EventAction> holyStrikeActions = new List<EventAction> ();
		holyStrikeActions.Add(EventAction.CreateAudioEventAction(Event.CAST_START, holyStrikeSFx, EventTarget.CASTER));
		abilities [0] = new HolyStrike (holyStrikeActions);

		List<EventAction> rallyActions = new List<EventAction> ();
		rallyActions.Add(EventAction.CreateAudioEventAction(Event.CAST_START, rallySfx, EventTarget.CASTER));
		rallyActions.Add(EventAction.CreateEffectEventAction(Event.CAST_START, rallyFx, EventTarget.TARGETUNIT));
		abilities [1] = new Rally (rallyActions);
	}

}
