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
	void Start () {
		List<EventAction> jadeSlamActions = new List<EventAction> ();
		jadeSlamActions.Add(EventAction.CreateAudioEventAction(Event.CAST_START, JadeSlamSfxCaster, EventTarget.CASTER));
		jadeSlamActions.Add(EventAction.CreateAudioEventAction(Event.CAST_END, JadeSlamSfxHit, EventTarget.TARGETUNIT));
		abilities [0] = new JadeSlam (jadeSlamActions);

		List<EventAction> sacredGroundActions = new List<EventAction> ();
		sacredGroundActions.Add(EventAction.CreateAudioEventAction(Event.CAST_START, JadeSlamSfxCaster, EventTarget.CASTER));
		sacredGroundActions.Add(EventAction.CreateEffectAtLocationEventAction(Event.CAST_START, sacredGroundFx));
		abilities [1] = new SacredGround (sacredGroundActions);
	}

}
