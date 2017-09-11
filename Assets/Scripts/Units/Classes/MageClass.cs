using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MageClass : UnitClass {

	[SerializeField]
	GameObject arcaneBoltProjectile;
	[SerializeField]
	AudioClip arcaneBoltSfx;

	[SerializeField]
	GameObject frostNovaFx;
	[SerializeField]
	AudioClip frostNovaCasterSfx;
	[SerializeField]
	AudioClip frostNovaAoeSfx;
	[SerializeField]
	AudioClip frostNovaHitSfx;

	// Use this for initialization
	void Start () {
		List<EventAction> arcaneBoltActions = new List<EventAction> ();
		arcaneBoltActions.Add(EventAction.CreateAudioEventAction(Event.CAST_START, arcaneBoltSfx, EventTarget.CASTER));
		arcaneBoltActions.Add(EventAction.CreateProjectileEventAction(Event.CAST_START, arcaneBoltProjectile, 12f, 0.7f));
		abilities [0] = new ArcaneBolt (arcaneBoltActions);

		List<EventAction> frostNovaActions = new List<EventAction> ();
		frostNovaActions.Add(EventAction.CreateAudioEventAction(Event.CAST_START, frostNovaCasterSfx, EventTarget.CASTER));
		frostNovaActions.Add(EventAction.CreateAudioEventAction(Event.CAST_START, frostNovaAoeSfx, EventTarget.CASTER));
		frostNovaActions.Add(EventAction.CreateAudioEventAction(Event.CAST_END, frostNovaHitSfx, EventTarget.TARGETUNIT));
		frostNovaActions.Add(EventAction.CreateEffectAtLocationEventAction(Event.CAST_START, frostNovaFx, 0.7f));
		abilities [1] = new FrostNova (frostNovaActions);
	}

}
