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
		SpawnUnit (0, 0, 5, 5, 1);
		SpawnUnit (1, 1, 4, 4, 0);
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

	public void RemoveUnit(UnitController unit) {
		unit.myNode.myUnit = null;
		units.Remove (unit);
	}

	public void StartTurn(int playersTurn) {
		foreach (UnitController unit in units) {
			if (unit.myTeam == playersTurn) {
				unit.NewTurn ();
			}
		}
	}

	public bool UnitAlreadySelected(UnitController unit) {
		return selectedUnit == unit;
	}

	public bool SelectUnit(UnitController unit) {
		if (!UnitAlreadySelected(unit)) {
			selectedUnit = unit;
			selectedUnit.SetSelected (true);
			map.highlighter.HighlightTile (unit.myNode, SquareTarget.NONE);
			return true;
		}
		return false;
	}

	public void DeselectUnit() {
		if (selectedUnit != null) {
			selectedUnit.SetSelected (false);
			selectedUnit = null;
			map.highlighter.UnhighlightTiles ();
		}
	}

	public void ShowMovement(UnitController unit) {
		Dictionary<Node, float> reachableTiles = map.pathfinder.findReachableTiles (unit.myNode, unit.myStats.Speed, unit.myStats.WalkingType, unit.myTeam);
		map.highlighter.UnhighlightTiles ();
		map.highlighter.HighlightTile (unit.myNode, SquareTarget.NONE);
		map.highlighter.HighlightTiles (reachableTiles.Keys.ToList(), SquareTarget.MOVEMENT);
	}

	public bool ShowAbility(int ability) {
		UnitClass unitClass;

		if (selectedUnit == null) {
			//TODO add error messages
			return false;
		}

		if (selectedUnit.myStats.ActionPoints <= 0) {
			//TODO add error messages
			return false;
		}

		unitClass = selectedUnit.GetComponent<UnitClass> ();

		if (!unitClass.CanUseAbility (ability)) {
			//TODO add error messages
			return false;
		}

		activeAbility = unitClass.abilities [ability];
		List<Node> attackableTiles = map.pathfinder.FindAttackableTiles (selectedUnit, activeAbility);
		map.highlighter.UnhighlightTiles ();
		map.highlighter.HighlightTile (selectedUnit.myNode, SquareTarget.NONE);
		map.highlighter.HighlightTiles (attackableTiles, SquareTarget.ATTACK);
		return true;

	}

	public bool AttackTile(Node targetNode) {
		if (!activeAbility.CanTargetTile (selectedUnit, targetNode)) {
			return false;
		}

		Vector2 direction = map.GetDirectionBetweenNodes (selectedUnit.myNode, targetNode);
		activeAbility.UseAbility (selectedUnit, targetNode, direction);
		selectedUnit.FaceDirection (direction);
		selectedUnit.SetAttacking (true);
		selectedUnit.myStats.ActionPoints--;

		return true;
	}

	public void MoveToTile(Node targetNode) {
		MovementPath path = map.pathfinder.getPathFromTile (targetNode);
		selectedUnit.SetPath (path);
		selectedUnit.myStats.HasMoved = true;
		selectedUnit.myNode.myUnit = null;
	}

	public void UnitFinishedMoving() {
		TurnManager turnManager = GetComponent<TurnManager> ();
		turnManager.FinishedMoving ();
	}

	public void UnitFinishedAttacking() {
		TurnManager turnManager = GetComponent<TurnManager> ();
		turnManager.FinishedAttacking ();
	}

	public void UnitDied(UnitController unit) {
		RemoveUnit (unit);
	}

	public bool PlayerOutOfActions(int team) {
		bool noActionsRemaining = true;

		foreach (UnitController unit in units) {
			if (unit.myTeam == team && (unit.myStats.ActionPoints > 0 || !unit.myStats.HasMoved)) {
				noActionsRemaining = false;
			}
		}

		return noActionsRemaining;

	}
}
