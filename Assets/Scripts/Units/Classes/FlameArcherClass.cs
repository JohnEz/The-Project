using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlameArcherClass : UnitClass {

	[SerializeField]
	GameObject flamingArrowProjectile;
	[SerializeField]
	AudioClip flamingArrowCastSfx;
	[SerializeField]
	AudioClip flamingArrowImpactSfx;

	public FlameArcherClass() {
		className = "Flame Archer";
	}

	// Use this for initialization
	public override void Initialise(UnitController caster) {
		List<EventAction> flamingArrowActions = new List<EventAction> ();
		flamingArrowActions.Add(EventAction.CreateAudioEventAction(Event.CAST_START, flamingArrowCastSfx, EventTarget.CASTER));
		flamingArrowActions.Add(EventAction.CreateAudioEventAction(Event.CAST_END, flamingArrowImpactSfx, EventTarget.TARGETUNIT));
		flamingArrowActions.Add(EventAction.CreateProjectileEventAction(Event.CAST_START, flamingArrowProjectile, 1200f, 1f));
		abilities.Add(new FlamingArrow (flamingArrowActions, caster));

		List<EventAction> combustionActions = new List<EventAction> ();
		combustionActions.Add(EventAction.CreateAudioEventAction(Event.CAST_START, flamingArrowCastSfx, EventTarget.CASTER));
		combustionActions.Add(EventAction.CreateAudioEventAction(Event.CAST_END, flamingArrowImpactSfx, EventTarget.TARGETUNIT));
		combustionActions.Add(EventAction.CreateProjectileEventAction(Event.CAST_START, flamingArrowProjectile, 1200f, 1f));
		abilities.Add(new Combustion (combustionActions, caster));

	}

}
