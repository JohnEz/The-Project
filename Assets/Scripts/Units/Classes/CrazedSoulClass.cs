using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrazedSoulClass : UnitClass {

    [SerializeField]
    public AudioClip abilitySound0;

    [SerializeField]
    public GameObject attackImpactFx;

	public CrazedSoulClass() {
		className = "Crazed Soul";
	}

	// Use this for initialization
	public override void Initialise(UnitController caster) {
		List<EventAction> dualSlashActions = new List<EventAction> ();
		dualSlashActions.Add(EventAction.CreateAudioEventAction(AbilityEvent.CAST_START, abilitySound0, EventTarget.CASTER));
        dualSlashActions.Add(EventAction.CreateEffectEventAction(AbilityEvent.CAST_END, attackImpactFx, EventTarget.TARGETUNIT));
		abilities.Add(new DualSlash (dualSlashActions, caster));
	}

}
