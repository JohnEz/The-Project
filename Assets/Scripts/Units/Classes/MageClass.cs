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
	AudioClip frostNovaSfx;

	// Use this for initialization
	void Start () {
		List<EventAction> arcaneBoltActions = new List<EventAction> ();
		arcaneBoltActions.Add(EventAction.CreateAudioEventAction(Event.CAST_START, arcaneBoltSfx, true));
		arcaneBoltActions.Add(EventAction.CreateProjectileEventAction(Event.CAST_START, arcaneBoltProjectile, 12f, 0.7f));
		abilities [0] = new ArcaneBolt (arcaneBoltActions);

		List<EventAction> frostNovaActions = new List<EventAction> ();
		frostNovaActions.Add(EventAction.CreateAudioEventAction(Event.CAST_START, frostNovaSfx, true));
		frostNovaActions.Add(EventAction.CreateEffectEventAction(Event.CAST_START, frostNovaFx, false));
		abilities [1] = new Cauterize (frostNovaActions);
	}

}
