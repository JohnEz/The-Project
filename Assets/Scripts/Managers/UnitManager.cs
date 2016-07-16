using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UnitManager : MonoBehaviour {

	List<UnitController> units;

	public GameObject[] unitPrefabs;

	TileMap map;

	//currentSelect
	UnitController selectedUnit = null;
	BaseAbility activeAbility = null;


	// Use this for initialization
	void Start () {

	}

	public void Initialise() {
		units = new List<UnitController> ();
		map = GetComponentInChildren<TileMap> ();
		map.Initialise ();
		SpawnUnit (0, 0, 0, 0, 1);
		SpawnUnit (0, 1, 5, 5, 1);
		SpawnUnit (0, 2, 4, 4, 0);
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
		unitController.Initialise ();
		units.Add (unitController);
	}

	public void StartTurn(int playersTurn) {
		foreach (UnitController unit in units) {
			if (unit.myTeam == playersTurn) {
				unit.NewTurn ();
			}
		}
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
		Dictionary<Node, float> reachableTiles = map.pathfinder.findReachableTiles (unit.myNode, unit.myStats.Speed, unit.myStats.WalkingType, unit.myTeam);
		map.highlighter.HighlightTiles (reachableTiles.Keys.ToList(), SquareTarget.MOVEMENT);
	}

	public bool ShowAbility(int ability) {
		UnitClass unitClass;

		if (selectedUnit == null) {
			Debug.Log ("For some reason the attacking unit is not selected");
			return false;
		} else {
			unitClass = selectedUnit.GetComponent<UnitClass> ();
		}

		if (unitClass.CanUseAbility(ability)) {
			activeAbility = unitClass.abilities [ability];
			List<Node> attackableTiles = map.pathfinder.FindAttackableTiles (selectedUnit, activeAbility);
			map.highlighter.HighlightTiles (attackableTiles, SquareTarget.ATTACK);
			return true;
		}
		return false;
	}

	public bool AttackTile(Node targetNode) {
		if (activeAbility.CanTargetTile(selectedUnit, targetNode)) {
			Vector2 direction = map.GetDirectionBetweenNodes (selectedUnit.myNode, targetNode);
			activeAbility.UseAbility (selectedUnit, targetNode, direction);
			selectedUnit.FaceDirection (direction);
			selectedUnit.SetAttacking (true);
			selectedUnit.myStats.ActionPoints--;
			return true;
		}
		return false;
	}

	public void MoveToTile(Node targetNode) {
		MovementPath path = map.pathfinder.getPathFromTile (targetNode);
		selectedUnit.SetPath (path);
		selectedUnit.myStats.ActionPoints--;
		selectedUnit.myNode.myUnit = null;
	}

	public void UnitFinishedMoving() {
		TurnManager turnManager = GetComponent<TurnManager> ();
		turnManager.FinishedMoving ();

		//check to see if that was the last thing the player could do
		if (PlayerOutOfActions (turnManager.playersTurn)) {
			turnManager.EndTurn ();
		}
	}

	public void UnitFinishedAttacking() {
		TurnManager turnManager = GetComponent<TurnManager> ();
		turnManager.FinishedAttacking ();

		//check to see if that was the last thing the player could do
		if (PlayerOutOfActions (turnManager.playersTurn)) {
			turnManager.EndTurn ();
		}
	}

	public bool PlayerOutOfActions(int team) {
		bool noActions = true;

		foreach (UnitController unit in units) {
			if (unit.myTeam == team && unit.myStats.ActionPoints > 0) {
				noActions = false;
			}
		}

		return noActions;

	}
}
