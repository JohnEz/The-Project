using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class AIManager : MonoBehaviour {

	UnitManager unitManager;

	List<UnitController> myUnits;

	TileMap myMap;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		
	}

	public void Initialise(TileMap map) {
		unitManager = GetComponent<UnitManager> ();
		myMap = map;
	}

	public List<UnitController> MyUnits {
		get { return myUnits; }
		set { myUnits = value; }
	}

	// NewTurn is called at the start of each of the AIs turns.
	public void NewTurn(int myPlayerId) {
		myUnits = unitManager.Units.Where (unit => unit.myPlayer.id == myPlayerId).ToList ();

		foreach (UnitController unit in myUnits) {
			unitManager.SelectUnit (unit);
			List<MovementPath> paths = FindPathsToEnemies (unit);

			MovementPath shortestPath = new MovementPath();
			shortestPath.movementCost = -1;

			foreach (MovementPath path in paths) {
				if (shortestPath.movementCost == -1 || path.movementCost < shortestPath.movementCost) {
					shortestPath = path;
				}
			}

			shortestPath.path.RemoveAt (shortestPath.path.Count - 1);
		}
	}

	public List<MovementPath> FindPathsToEnemies(UnitController unit) {
		List<MovementPath> pathsToEnemies = new List<MovementPath> ();

		foreach (UnitController otherUnit in unitManager.Units) {
			if (otherUnit.myPlayer.faction != unit.myPlayer.faction) {
				MovementPath pathToEnemy = myMap.pathfinder.FindPath (unit.myNode, otherUnit.myNode, unit.myStats.walkingType, unit.myPlayer.faction);
				pathsToEnemies.Add (pathToEnemy);
			}
		}

		return pathsToEnemies;
	}


}
