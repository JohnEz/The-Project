﻿using UnityEngine;
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

	public MageClass() {
		className = "Mage";
	}

	// Use this for initialization
	public override void Initialise (UnitController caster) {
		List<EventAction> arcaneBoltActions = new List<EventAction> ();
		arcaneBoltActions.Add(EventAction.CreateAudioEventAction(AbilityEvent.CAST_START, arcaneBoltSfxCaster, EventTarget.CASTER));
		arcaneBoltActions.Add(EventAction.CreateAudioEventAction(AbilityEvent.CAST_END, arcaneBoltSfxHit, EventTarget.TARGETUNIT));
		arcaneBoltActions.Add(EventAction.CreateProjectileEventAction(AbilityEvent.CAST_START, arcaneBoltProjectile, 1200f, 0.7f));
		abilities.Add(new ArcaneBolt (arcaneBoltActions, caster));

		List<EventAction> frostNovaActions = new List<EventAction> ();
		frostNovaActions.Add(EventAction.CreateAudioEventAction(AbilityEvent.CAST_START, frostNovaCasterSfx, EventTarget.CASTER));
		frostNovaActions.Add(EventAction.CreateAudioEventAction(AbilityEvent.CAST_START, frostNovaAoeSfx, EventTarget.CASTER));
		frostNovaActions.Add(EventAction.CreateEffectAtLocationEventAction(AbilityEvent.CAST_START, frostNovaFx, 0.7f));
		abilities.Add(new FrostNova (frostNovaActions, frostNovaDebuffFx, caster));
	}

}
