﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ObjectiveType {
	ANNIHILATION
}

public struct Objective {
	public ObjectiveType type;
	public string text;
	public bool optional;
}

public class ObjectiveManager : MonoBehaviour
{

	private Dictionary<Player, List<Objective>> objectives = new Dictionary<Player, List<Objective>>();
	private UnitManager unitManager;

	// Use this for initialization
	public void Initialise () {
		unitManager = GetComponent<UnitManager> ();
	}

	public List<Objective> getObjectives(Player player) {
		return objectives [player];
	}

	public void AddObjective(Player player, Objective objective) {
		if (!objectives.ContainsKey (player)) {
			List<Objective> newObjectives = new List<Objective> ();
			newObjectives.Add (objective);
			objectives.Add (player, newObjectives);
		} else {
			objectives [player].Add (objective);
		}
	}

	public void RemoveObjective() {

	}

	public bool CheckObjectives(Player player) {
		if (objectives.ContainsKey (player)) {
			List<Objective> playerObjectives = objectives [player];

			foreach (Objective objective in playerObjectives) {
				bool objectiveCompleted;
				switch (objective.type) {
				case ObjectiveType.ANNIHILATION:
					objectiveCompleted = Annihilation (player);
					break;
				default:
					objectiveCompleted = true;
					break;
				}

				if (!objectiveCompleted) {
					return false;
				}
			}

			return true;
		}
		return false;
	}

	private bool Annihilation(Player player) {
		List<UnitController> units = unitManager.Units;

		foreach (UnitController unit in units) {
			if (unit.myPlayer.faction != player.faction) {
				return false;
			}
		}

		return true;
	}
}
