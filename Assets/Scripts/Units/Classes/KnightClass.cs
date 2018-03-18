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
	public override void Initialise(UnitStats casterStats) {
		List<EventAction> holyStrikeActions = new List<EventAction> ();
		holyStrikeActions.Add(EventAction.CreateAudioEventAction(Event.CAST_START, holyStrikeSFx, EventTarget.CASTER));
		abilities.Add(new HolyStrike (holyStrikeActions, casterStats));

		List<EventAction> rallyActions = new List<EventAction> ();
		rallyActions.Add(EventAction.CreateAudioEventAction(Event.CAST_START, rallySfx, EventTarget.CASTER));
		rallyActions.Add(EventAction.CreateEffectEventAction(Event.CAST_START, rallyFx, EventTarget.TARGETUNIT));
		abilities.Add(new Rally (rallyActions, casterStats));
	}

}
