using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TargetType {
	ENEMY,
	ALLY,
	UNIT //both enemies and allies
}

public enum TileTarget {
	TILE,
	UNIT
}

public enum AreaOfEffect {
	SINGLE,
	CIRCLE,
	CLEAVE,
	AURA
}

public enum Event {
	CAST_START,
	CAST_END, //called just for the caster after cast
	HIT //called for each target on hit
}

public enum EventTarget {
	TARGETEDTILE,
	CASTER,
	TARGETUNIT
}

public struct EventAction {
	public Event eventTrigger;
	public EventTarget eventTarget;
	public System.Action<UnitController, UnitController, Node> action;

	public static EventAction CreateAudioEventAction(Event _eventTrigger, AudioClip audioClip, EventTarget _eventTarget) {
		EventAction newEventAction = new EventAction ();
		newEventAction.eventTrigger = _eventTrigger;
		newEventAction.eventTarget = _eventTarget;
		newEventAction.action = (UnitController caster, UnitController target, Node targetedTile) => {
			switch (_eventTarget) {
			case EventTarget.CASTER:
				caster.PlayOneShot(audioClip);
				break;
			case EventTarget.TARGETUNIT:
				target.PlayOneShot(audioClip);
				break;
			case EventTarget.TARGETEDTILE:
				//TODO ADD SOUNDEFFECT TO NODE
				break;
			};
		};

		return newEventAction;
	}

	public static EventAction CreateEffectEventAction(Event _eventTrigger, GameObject effectObject, EventTarget _eventTarget, float delay = 0) {
		EventAction newEventAction = new EventAction ();
		newEventAction.eventTrigger = _eventTrigger;
		newEventAction.eventTarget = _eventTarget;
		newEventAction.action = (UnitController caster, UnitController target, Node targetedTile) => {
			switch (_eventTarget) {
			case EventTarget.CASTER:
				caster.CreateEffectWithDelay(effectObject, delay);
				break;
			case EventTarget.TARGETUNIT:
				target.CreateEffectWithDelay(effectObject, delay);
				break;
			};
		};

		return newEventAction;
	}

	public static EventAction CreateEffectAtLocationEventAction(Event _eventTrigger, GameObject effectObject, float delay = 0) {
		EventAction newEventAction = new EventAction ();
		newEventAction.eventTrigger = _eventTrigger;
		newEventAction.eventTarget = EventTarget.TARGETEDTILE;
		newEventAction.action = (UnitController caster, UnitController target, Node targetedTile) => {
			caster.CreateEffectWithDelay(effectObject, delay, targetedTile);
		};

		return newEventAction;
	}

	public static EventAction CreateProjectileEventAction(Event _eventTrigger, GameObject projectileObject, float speed, float delay = 0) {
		EventAction newEventAction = new EventAction ();
		newEventAction.eventTrigger = _eventTrigger;
		newEventAction.eventTarget = EventTarget.CASTER;
		newEventAction.action = (UnitController caster, UnitController target, Node targetedTile) => {
			caster.CreateProjectileWithDelay(projectileObject, target.myNode, speed, delay);
		};

		return newEventAction;
	}
}

public class BaseAbility {

	public int maxCooldown = 0;
	public int cooldown = 0;

	public int range = 1;
	public int minRange = 1;

	public AreaOfEffect areaOfEffect = AreaOfEffect.SINGLE;
	public int aoeRange = 1;

	public TargetType targets = TargetType.ENEMY;
	public TileTarget tileTarget = TileTarget.UNIT;

	public List<EventAction> eventActions;

	public string icon = "abilityHolyStrikeController";

	public BaseAbility(List<EventAction> _eventActions) {
		eventActions = _eventActions;
	}

	public bool IsOnCooldown() {
		return cooldown > 0;
	}

	public void NewTurn() {
		if (cooldown > 0) {
			cooldown--;
		}
	}

	public virtual void UseAbility(UnitController caster, Node target) {
		cooldown = maxCooldown;

		eventActions.ForEach ((eventAction) => {
			if (eventAction.eventTrigger == Event.CAST_START) {
				eventAction.action(caster, target.myUnit, target);
			}
		});
	}

	public virtual void UseAbility(UnitController caster, List<Node> targets, Node target) {
		cooldown = maxCooldown;

		eventActions.ForEach ((eventAction) => {
			if (eventAction.eventTrigger == Event.CAST_START) {
				if (eventAction.eventTarget == EventTarget.CASTER || eventAction.eventTarget == EventTarget.TARGETEDTILE) {
					eventAction.action(caster, null, target);
				} else if (eventAction.eventTarget == EventTarget.TARGETUNIT) {
					targets.ForEach((targetNode) => {
						if (CanHitUnit(caster, targetNode)) {
							eventAction.action(caster, targetNode.myUnit, target);
						}
					});
				}
			}
		});

	}

	public bool CanTargetTile(UnitController caster, Node targetNode) {
		if (tileTarget == TileTarget.UNIT && !CanHitUnit (caster, targetNode)) {
			return false;
		}

		return true;
	}

	public bool CanHitUnit(UnitController caster, Node targetNode) {
		//TODO this is dumb and needs rewriting

		if (targets == TargetType.ENEMY & (targetNode.myUnit == null || targetNode.myUnit.myPlayer.faction == caster.myPlayer.faction)) {
			return false;
		}

		if (targets == TargetType.ALLY & (targetNode.myUnit == null || targetNode.myUnit.myPlayer.faction != caster.myPlayer.faction)) {
			return false;
		}

		if (targets == TargetType.UNIT & targetNode.myUnit == null) {
			return false;
		}

		return true;
	}

	public void AddAbilityTarget(UnitController caster, UnitController target, System.Action ability) {
		caster.AddAbilityTarget (target, ability);
	}

	public int Cooldown {
		get { return cooldown; }
		set { cooldown = value; }
	}

	public int MaxCooldown {
		get { return maxCooldown; }
		set { maxCooldown = value; }
	}

}
