using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StaffMonkClass : UnitClass {

	[SerializeField]
	AudioClip JadeSlamSfxCaster;
	[SerializeField]
	AudioClip JadeSlamSfxHit;

	[SerializeField]
	GameObject sacredGroundFx;

	// Use this for initialization
	public override void Initialise(UnitStats casterStats) {
		List<EventAction> jadeSlamActions = new List<EventAction> ();
		jadeSlamActions.Add(EventAction.CreateAudioEventAction(Event.CAST_START, JadeSlamSfxCaster, EventTarget.CASTER));
		jadeSlamActions.Add(EventAction.CreateAudioEventAction(Event.CAST_END, JadeSlamSfxHit, EventTarget.TARGETUNIT));
		abilities.Add(new JadeSlam (jadeSlamActions, casterStats));

		List<EventAction> sacredGroundActions = new List<EventAction> ();
		sacredGroundActions.Add(EventAction.CreateAudioEventAction(Event.CAST_START, JadeSlamSfxCaster, EventTarget.CASTER));
		sacredGroundActions.Add(EventAction.CreateEffectAtLocationEventAction(Event.CAST_START, sacredGroundFx));
		abilities.Add(new SacredGround (sacredGroundActions, casterStats));
	}

}
