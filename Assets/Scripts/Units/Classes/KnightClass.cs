using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KnightClass : UnitClass {
	
	[SerializeField]
	AudioClip holyStrikeSfx;
    [SerializeField]
    GameObject holyStrikeFx;

    [SerializeField]
	GameObject rallyFx;
	[SerializeField]
	AudioClip rallySfx;

	public KnightClass() {
		className = "Knight";
	}

	// Use this for initialization
	public override void Initialise(UnitController caster) {
		List<EventAction> holyStrikeActions = new List<EventAction> ();
		holyStrikeActions.Add(EventAction.CreateAudioEventAction(AbilityEvent.CAST_START, holyStrikeSfx, EventTarget.CASTER));
        holyStrikeActions.Add(EventAction.CreateEffectEventAction(AbilityEvent.CAST_END, holyStrikeFx, EventTarget.TARGETUNIT));
        abilities.Add(new HolyStrike (holyStrikeActions, caster));

		List<EventAction> rallyActions = new List<EventAction> ();
		rallyActions.Add(EventAction.CreateAudioEventAction(AbilityEvent.CAST_START, rallySfx, EventTarget.CASTER));
		rallyActions.Add(EventAction.CreateEffectEventAction(AbilityEvent.CAST_START, rallyFx, EventTarget.TARGETUNIT));
		abilities.Add(new Rally (rallyActions, caster));
	}

}
