using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlamePriestClass : UnitClass {

	[SerializeField]
	GameObject engulfFx;
	[SerializeField]
	AudioClip engulfSfx;

	[SerializeField]
	GameObject cauterizeFx;
	[SerializeField]
	AudioClip cauterizeSfx;

	public FlamePriestClass() {
		className = "Flame Priest";
	}

	// Use this for initialization
	public override void Initialise(UnitController caster) {
		List<EventAction> englufActions = new List<EventAction> ();
		englufActions.Add(EventAction.CreateAudioEventAction(Event.CAST_START, engulfSfx, EventTarget.CASTER));
		englufActions.Add(EventAction.CreateEffectEventAction(Event.CAST_START, engulfFx, EventTarget.TARGETUNIT, 0.5f));
		abilities.Add(new Engulf (englufActions, caster));

		List<EventAction> cauterizeActions = new List<EventAction> ();
		cauterizeActions.Add(EventAction.CreateAudioEventAction(Event.CAST_START, cauterizeSfx, EventTarget.CASTER));
		cauterizeActions.Add(EventAction.CreateEffectEventAction(Event.CAST_START, cauterizeFx, EventTarget.TARGETUNIT, 0.5f));
		abilities.Add(new Cauterize (cauterizeActions, caster));
	}

}
