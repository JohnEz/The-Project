using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MageClass : UnitClass {

	[SerializeField]
	GameObject arcaneBoltProjectile;
	[SerializeField]
	AudioClip arcaneBoltSfxCaster;
	[SerializeField]
	AudioClip arcaneBoltSfxHit;

	[SerializeField]
	GameObject frostNovaFx;
	[SerializeField]
	GameObject frostNovaDebuffFx;
	[SerializeField]
	AudioClip frostNovaCasterSfx;
	[SerializeField]
	AudioClip frostNovaAoeSfx;

	// Use this for initialization
	public override void Initialise () {
		List<EventAction> arcaneBoltActions = new List<EventAction> ();
		arcaneBoltActions.Add(EventAction.CreateAudioEventAction(Event.CAST_START, arcaneBoltSfxCaster, EventTarget.CASTER));
		arcaneBoltActions.Add(EventAction.CreateAudioEventAction(Event.CAST_END, arcaneBoltSfxHit, EventTarget.TARGETUNIT));
		arcaneBoltActions.Add(EventAction.CreateProjectileEventAction(Event.CAST_START, arcaneBoltProjectile, 12f, 0.7f));
		abilities.Add(new ArcaneBolt (arcaneBoltActions));

		List<EventAction> frostNovaActions = new List<EventAction> ();
		frostNovaActions.Add(EventAction.CreateAudioEventAction(Event.CAST_START, frostNovaCasterSfx, EventTarget.CASTER));
		frostNovaActions.Add(EventAction.CreateAudioEventAction(Event.CAST_START, frostNovaAoeSfx, EventTarget.CASTER));
		frostNovaActions.Add(EventAction.CreateEffectAtLocationEventAction(Event.CAST_START, frostNovaFx, 0.7f));
		abilities.Add(new FrostNova (frostNovaActions, frostNovaDebuffFx));
	}

}
