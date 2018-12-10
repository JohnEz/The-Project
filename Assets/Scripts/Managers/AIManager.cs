﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class AIManager : MonoBehaviour {

    public static AIManager singleton;

	List<UnitController> myUnits;

	TileMap myMap;

    private void Awake() {
        singleton = this;
    }

    // Use this for initialization
    void Start () {

	}

	// Update is called once per frame
	void Update () {
		
	}

	public void Initialise(TileMap map) {
		myMap = map;
	}

	public List<UnitController> MyUnits {
		get { return myUnits; }
		set { myUnits = value; }
	}

	// NewTurn is called at the start of each of the AIs turns.
	public IEnumerator NewTurn(int myPlayerId) {
		myUnits = UnitManager.singleton.Units.Where (unit => unit.myPlayer.id == myPlayerId).ToList ();

		foreach (UnitController unit in myUnits) {
			yield return PlanTurn (unit);
		}

		TurnManager.singleton.EndTurn ();
	}

	public List<MovementPath> FindPathsToEnemies(UnitController unit) {
		List<MovementPath> pathsToEnemies = new List<MovementPath> ();

		foreach (UnitController otherUnit in UnitManager.singleton.Units) {
			if (otherUnit.myPlayer.faction != unit.myPlayer.faction) {
				MovementPath pathToEnemy = myMap.pathfinder.FindShortestPathToUnit (unit.myNode, otherUnit.myNode, unit.myStats.walkingType, unit.myPlayer.faction);
				if (pathToEnemy.movementCost != -1) {
					pathsToEnemies.Add (pathToEnemy);
				}
			}
		}

		return pathsToEnemies;
	}

	public Dictionary<AbilityCardBase, List<Node>> GetPotentialAbilityTargets(UnitController unit) {
		Dictionary<AbilityCardBase, List<Node>> potentialAbilityTargets = new Dictionary<AbilityCardBase, List<Node>>();
		//UnitClass unitClass = unit.GetComponent<UnitClass>();

		//unitClass.abilities.ForEach (ability => {
		//	List<Node> attackableTiles = myMap.pathfinder.FindAttackableTiles (unit.myNode, ability);

		//	attackableTiles = attackableTiles.Where(tile => ability.CanHitUnit(tile)).ToList();

		//	potentialAbilityTargets.Add(ability, attackableTiles);
		//});

		return potentialAbilityTargets;
	}

	public IEnumerator PlanTurn(UnitController unit) {
		// UnitClass unitClass = unit.GetComponent<UnitClass>();
        // unitManager.SelectUnit (unit);

        Debug.Log("AI - Planning turn");

  //      while (unit.myStats.ActionPoints > 0) {
		//	yield return WaitForWaitingForInput ();
		//	unitManager.SelectUnit (unit);

		//	Dictionary<AbilityCardBase, List<Node>> potentialAbilityTargets = GetPotentialAbilityTargets(unit);

		//	if (potentialAbilityTargets [unitClass.abilities [0]].Count > 0) {
  //              Debug.Log("AI - attacking tile");
  //              unitManager.AttackTile (potentialAbilityTargets [unitClass.abilities [0]] [0], unitClass.abilities [0]);
		//	} else if (unit.myStats.Speed > 0) {
		//		List<MovementPath> paths = FindPathsToEnemies (unit);
		//		if (paths.Count > 0) {
		//			MovementPath shortestPath = Pathfinder.GetSortestPath (paths);
		//			//TODO we need to check if there is a unit on the tile speed away
		//			shortestPath.path = shortestPath.path.Take (unit.myStats.Speed * unit.myStats.ActionPoints).ToList ();
		//			unitManager.SetUnitPath (shortestPath);
		//		} else {
		//			//end turn
		//			unit.ActionPoints = 0;
		//		}
		//	} else {
		//		//end turn
		//		unit.ActionPoints = 0;
		//	}
		//	unitManager.DeselectUnit ();
		//}
			
		yield return WaitForWaitingForInput ();

	}

	public IEnumerator WaitForWaitingForInput() {
		return new WaitUntil (() => TurnManager.singleton.CurrentPhase == TurnPhase.WAITING_FOR_INPUT);
	}
}
