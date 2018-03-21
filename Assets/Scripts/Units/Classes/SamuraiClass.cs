using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SamuraiClass : UnitClass {

	[SerializeField]
	public AudioClip samuraiAttack;
	[SerializeField]
	public AudioClip samuraiAttackHit;

	[SerializeField]
	GameObject slashFx;

	public SamuraiClass() {
		className = "Samurai";
	}

	// Use this for initialization
	public override void Initialise(UnitStats casterStats) {
		List<EventAction> omniSlashActions = new List<EventAction> ();
		omniSlashActions.Add(EventAction.CreateAudioEventAction(Event.CAST_START, samuraiAttack, EventTarget.CASTER));
		omniSlashActions.Add(EventAction.CreateAudioEventAction(Event.HIT, samuraiAttackHit, EventTarget.TARGETUNIT));
		omniSlashActions.Add(EventAction.CreateEffectEventAction(Event.CAST_START, slashFx, EventTarget.TARGETUNIT, 0.55f));
		abilities.Add(new OmniSlash (omniSlashActions, casterStats));

		List<EventAction> cleaveActions = new List<EventAction> ();
		cleaveActions.Add(EventAction.CreateAudioEventAction(Event.CAST_START, samuraiAttack, EventTarget.CASTER));
		cleaveActions.Add(EventAction.CreateAudioEventAction(Event.HIT, samuraiAttackHit, EventTarget.TARGETUNIT));
		cleaveActions.Add(EventAction.CreateEffectEventAction(Event.CAST_START, slashFx, EventTarget.TARGETUNIT, 0.55f));
		abilities.Add(new Cleave (cleaveActions, casterStats));
	}

}
