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

	public StaffMonkClass() {
		className = "Jade Monk";
	}

	// Use this for initialization
	public override void Initialise(UnitController caster) {
		List<EventAction> jadeSlamActions = new List<EventAction> ();
		jadeSlamActions.Add(EventAction.CreateAudioEventAction(AbilityEvent.CAST_START, JadeSlamSfxCaster, EventTarget.CASTER));
		jadeSlamActions.Add(EventAction.CreateAudioEventAction(AbilityEvent.CAST_END, JadeSlamSfxHit, EventTarget.TARGETUNIT));
		abilities.Add(new JadeSlam (jadeSlamActions, caster));

		List<EventAction> sacredGroundActions = new List<EventAction> ();
		sacredGroundActions.Add(EventAction.CreateAudioEventAction(AbilityEvent.CAST_START, JadeSlamSfxCaster, EventTarget.CASTER));
		sacredGroundActions.Add(EventAction.CreateEffectAtLocationEventAction(AbilityEvent.CAST_START, sacredGroundFx));
		abilities.Add(new SacredGround (sacredGroundActions, caster));
	}

}
