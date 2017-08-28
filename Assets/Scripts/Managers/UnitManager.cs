using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UnitManager : MonoBehaviour {

	List<UnitController> units;

	public GameObject[] unitPrefabs;

	TileMap myMap;

	//currentSelect
	UnitController selectedUnit = null;
	BaseAbility activeAbility = null;


	// Use this for initialization
	void Start () {

	}

	public void Initialise(List<Player> players, TileMap map) {
		units = new List<UnitController> ();
		myMap = map;
		SpawnUnit (0, players[0], 0, 0);
		SpawnUnit (0, players[0], 3, 5);
		SpawnUnit (1, players[0], 4, 4);
		SpawnUnit (2, players[1], 4, 5);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public List<UnitController> Units {
		get { return units; }
	}

	public void SpawnUnit(int unit, Player player, int x, int y) {
		GameObject newUnit = (GameObject)Instantiate (unitPrefabs [unit], myMap.getPositionOfNode (x, y), Quaternion.identity);
		newUnit.transform.parent = myMap.transform;

		Node startingNode = myMap.getNode (x, y);
		UnitController unitController = newUnit.GetComponent<UnitController> ();

		startingNode.myUnit = unitController;
		unitController.Spawn(player, startingNode);
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
			if (unit.myPlayer.id == playersTurn) {
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
			myMap.highlighter.HighlightTile (unit.myNode, SquareTarget.NONE);
			return true;
		}
		return false;
	}

	public void DeselectUnit() {
		if (selectedUnit != null) {
			selectedUnit.SetSelected (false);
			selectedUnit = null;
			myMap.highlighter.UnhighlightTiles ();
		}
	}

	public ReachableTiles FindReachableTiles(UnitController unit) {
		return myMap.pathfinder.findReachableTiles (unit.myNode, unit.myStats.Speed, unit.myStats.WalkingType, unit.myPlayer.faction);
	}

	public void ShowActions(UnitController unit) {
		myMap.highlighter.UnhighlightTiles ();
		myMap.highlighter.HighlightTile (unit.myNode, SquareTarget.NONE);

		UnitClass unitClass = unit.GetComponent<UnitClass>();
		MovementAndAttackPath reachableTiles = myMap.pathfinder.findMovementAndAttackTiles (unit, unitClass.abilities [0], unit.myStats.ActionPoints);
		activeAbility = unitClass.abilities[0];

		myMap.highlighter.HighlightTiles (reachableTiles.movementTiles.basic.Keys.ToList(), SquareTarget.MOVEMENT);
		myMap.highlighter.HighlightTiles (reachableTiles.movementTiles.extended.Keys.ToList(), SquareTarget.DASH);
		myMap.highlighter.HighlightTiles (reachableTiles.attackTiles, SquareTarget.ATTACK);
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
		List<Node> attackableTiles = myMap.pathfinder.FindAttackableTiles (selectedUnit.myNode, activeAbility);
		myMap.highlighter.UnhighlightTiles ();
		myMap.highlighter.HighlightTile (selectedUnit.myNode, SquareTarget.NONE);
		myMap.highlighter.HighlightTiles (attackableTiles, SquareTarget.ATTACK);

		return true;
	}

	public bool AttackTile(Node targetNode) {
		if (!activeAbility.CanTargetTile (selectedUnit, targetNode)) {
			return false;
		}

		MovementPath movementPath = myMap.pathfinder.getPathFromTile (targetNode.previous.node);
		Action attackAction = new Action();
		attackAction.type = ActionType.ATTACK;
		attackAction.ability = activeAbility;
		attackAction.nodes = new List<Node> ();
		attackAction.nodes.Add (targetNode);

		if (movementPath.movementCost >= 1) {
			SetUnitPath (movementPath);
		}

		selectedUnit.AddAction (attackAction);

		return true;
	}

	public void SetUnitPath(MovementPath movementPath) {
		Action moveAction = new Action();
		moveAction.type = ActionType.MOVEMENT;
		moveAction.nodes = movementPath.path;
		selectedUnit.AddAction (moveAction);
	}

	public void MoveToTile(Node targetNode) {
		MovementPath movementPath = myMap.pathfinder.getPathFromTile (targetNode);
		SetUnitPath(movementPath);
	}

	public void UnitStartedMoving() {
		TurnManager turnManager = GetComponent<TurnManager> ();
		turnManager.StartMoving ();
	}

	public void UnitFinishedMoving() {
		TurnManager turnManager = GetComponent<TurnManager> ();
		turnManager.FinishedMoving ();
	}

	public void UnitStartedAttacking() {
		TurnManager turnManager = GetComponent<TurnManager> ();
		turnManager.StartAttacking ();
	}

	public void UnitFinishedAttacking() {
		TurnManager turnManager = GetComponent<TurnManager> ();
		turnManager.FinishedAttacking ();
	}

	public void UnitDied(UnitController unit) {
		RemoveUnit (unit);
	}

	public bool PlayerOutOfActions(int playerId) {
		foreach (UnitController unit in units) {
			if (unit.myPlayer.id == playerId && unit.myStats.ActionPoints > 0) {
				return false;
			}
		}
		return true;
	}
}
