using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuardianClass : UnitClass {

	[SerializeField]
	AudioClip slamSfx;
	[SerializeField]
	AudioClip slamImpactSfx;
    [SerializeField]
    GameObject slamImpactFx;

    [SerializeField]
    AudioClip demoralisingShoutSfx;
    [SerializeField]
    GameObject demoralisingShoutFx;

    public GuardianClass() {
		className = "Guardian";
	}

	// Use this for initialization
	public override void Initialise(UnitController caster) {
		List<EventAction> shieldSlamActions = new List<EventAction> ();
		shieldSlamActions.Add(EventAction.CreateAudioEventAction(AbilityEvent.CAST_START, slamSfx, EventTarget.CASTER));
        shieldSlamActions.Add(EventAction.CreateEffectEventAction(AbilityEvent.CAST_END, slamImpactFx, EventTarget.TARGETUNIT));
        shieldSlamActions.Add(EventAction.CreateAudioEventAction(AbilityEvent.CAST_END, slamImpactSfx, EventTarget.CASTER));
		abilities.Add(new ShieldSlam (shieldSlamActions, caster));

		List<EventAction> hunkerDownActions = new List<EventAction> ();
		hunkerDownActions.Add(EventAction.CreateAudioEventAction(AbilityEvent.CAST_END, slamImpactSfx, EventTarget.CASTER));
		abilities.Add(new HunkerDown (hunkerDownActions, caster));

        List<EventAction> demoralisingShoutActions = new List<EventAction>();
        demoralisingShoutActions.Add(EventAction.CreateAudioEventAction(AbilityEvent.CAST_START, demoralisingShoutSfx, EventTarget.CASTER));
        demoralisingShoutActions.Add(EventAction.CreateEffectEventAction(AbilityEvent.CAST_START, demoralisingShoutFx, EventTarget.TARGETUNIT));
        abilities.Add(new DemoralisingShout(demoralisingShoutActions, caster));
    }

}


