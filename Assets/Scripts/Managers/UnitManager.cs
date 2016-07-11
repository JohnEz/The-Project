using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UnitManager : MonoBehaviour {

	List<UnitController> units;

	public GameObject[] unitPrefabs;

	TileMap map;

	UnitController selectedUnit = null;

	int playerCount = 2;

	// Use this for initialization
	void Start () {
		units = new List<UnitController> ();
		map = GetComponentInChildren<TileMap> ();
		map.Initialise ();
		SpawnUnit (0, 0, 0, 0, 1);
		SpawnUnit (0, 0, 5, 5, 1);
		SpawnUnit (0, 1, 4, 4, 0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SpawnUnit(int unit, int team, int x, int y, int player) {
		GameObject newUnit = (GameObject)Instantiate (unitPrefabs [unit], map.getPositionOfNode (x, y), Quaternion.identity);
		newUnit.transform.parent = map.transform;

		Node startingNode = map.getNode (x, y);
		UnitController unitController = newUnit.GetComponent<UnitController> ();

		startingNode.myUnit = unitController;
		unitController.Spawn(team, player, startingNode);
		unitController.FaceDirection (Vector2.down);
		unitController.myManager = this;
		units.Add (unitController);
	}

	public bool SelectUnit(UnitController unit) {
		if (selectedUnit != unit) {
			selectedUnit = unit;
			return true;
		}
		return false;
	}

	public void DeselectUnit() {
		selectedUnit = null;
		map.highlighter.UnhighlightTiles ();
	}

	public void ShowMovement(UnitController unit) {
		Dictionary<Node, float> reachableTiles = map.pathfinder.findReachableTiles (unit.myNode, unit.movementSpeed, unit.walkingType, unit.myTeam);
		map.highlighter.HighlightTiles (reachableTiles.Keys.ToList(), SquareTarget.MOVEMENT);
	}

	public void MoveToTile(Node targetNode) {
		MovementPath path = map.pathfinder.getPathFromTile (targetNode);
		selectedUnit.SetPath (path);
		selectedUnit.myNode.myUnit = null;
		selectedUnit.actionPoints--;
	}

	public void UnitFinishedMoving() {
		GetComponent<TurnManager> ().FinishedMoving ();
	}
}
