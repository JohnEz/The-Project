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
	public UnitController lastSelectedUnit = null;
	BaseAbility activeAbility = null;
	List<Node> attackableTiles = new List<Node>();


	// Use this for initialization
	void Start () {

	}

	public void Initialise(List<Player> players, TileMap map) {
		units = new List<UnitController> ();
		myMap = map;
		SpawnUnit (5, players[0], 3, 8);
		SpawnUnit (3, players[0], 3, 9);
		SpawnUnit (4, players[0], 3, 10);
		SpawnUnit (2, players[1], 9, 9);
		SpawnUnit (1, players[1], 9, 10);
		SpawnUnit (2, players[1], 8, 10);
		SpawnUnit (2, players[1], 9, 11);
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

	public void EndTurn(int playersTurn) {
		lastSelectedUnit = null;
		foreach (UnitController unit in units) {
			if (unit.myPlayer.id == playersTurn) {
				unit.EndTurn ();
			}
		}
	}

	public bool UnitAlreadySelected(UnitController unit) {
		return selectedUnit == unit;
	}

	public bool SelectUnit(UnitController unit) {
		if (!UnitAlreadySelected(unit)) {
			selectedUnit = unit;
			lastSelectedUnit = unit;
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
			activeAbility = null;
			attackableTiles = new List<Node> ();
			myMap.highlighter.UnhighlightAllTiles ();
		}
	}

	public ReachableTiles FindReachableTiles(UnitController unit) {
		return myMap.pathfinder.findReachableTiles (unit.myNode, unit.myStats.Speed, unit.myStats.WalkingType, unit.myPlayer.faction);
	}

	public void ShowActions(UnitController unit = null) {
		unit = unit == null ? selectedUnit : unit;

		myMap.highlighter.UnhighlightAllTiles ();
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
		attackableTiles = myMap.pathfinder.FindAttackableTiles (selectedUnit.myNode, activeAbility);
		myMap.highlighter.UnhighlightAllTiles ();
		myMap.highlighter.HighlightTile (selectedUnit.myNode, SquareTarget.NONE);
		SquareTarget targetType = activeAbility.targets == TargetType.ALLY ? SquareTarget.HELPFULL : SquareTarget.ATTACK;
		myMap.highlighter.HighlightTiles (attackableTiles, targetType);

		return true;
	}

	public void HighlightEffectedTiles(Node target) {
		if (attackableTiles.Contains (target)) {
			List<Node> effectedNodes = GetTargetNodes (activeAbility, target);
			SquareTarget targetType = activeAbility.targets == TargetType.ALLY ? SquareTarget.HELPFULL : SquareTarget.ATTACK;
			myMap.highlighter.SetEffectedTiles (effectedNodes, targetType);
		}
	}

	public void UnhiglightEffectedTiles() {
		myMap.highlighter.ClearEffectedTiles ();
	}

	public void ShowPath(Node targetNode) {
		MovementPath movementPath = myMap.pathfinder.getPathFromTile (targetNode);
		myMap.highlighter.SetEffectedTiles (movementPath.path);
	}

	public bool AttackTile(Node targetNode) {
		if (!activeAbility.CanTargetTile (selectedUnit, targetNode)) {
			return false;
		}

		if (targetNode.previousMoveNode) {
			MovementPath movementPath = myMap.pathfinder.getPathFromTile (targetNode.previousMoveNode);
			SetUnitPath (movementPath);
		}

		Action attackAction = new Action();
		attackAction.type = ActionType.ATTACK;
		attackAction.ability = activeAbility;
		attackAction.nodes = GetTargetNodes(activeAbility, targetNode);

		selectedUnit.AddAction (attackAction);

		return true;
	}

	List<Node> GetTargetNodes(BaseAbility ability, Node targetNode) {
		List<Node> targetTiles = new List<Node> ();
		switch (ability.areaOfEffect) {
		case AreaOfEffect.AURA:
			return attackableTiles;
		case AreaOfEffect.CIRCLE:
			return myMap.pathfinder.FindAOEHitTiles(targetNode, ability);
		case AreaOfEffect.SINGLE:
		default:
			targetTiles.Add (targetNode);
			return targetTiles;
		}
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

	public UnitController GetNextUnit (int player, bool withActions = true) {
		if (selectedUnit == null) {
			return GetFirstUnit (player, withActions);
		} else {
			int startingIndex = units.FindIndex ((unit) => unit == selectedUnit);
			int currentIndex = (startingIndex + 1) % units.Count;

			while (currentIndex != startingIndex) {
				UnitController unit = units [currentIndex];
				if (unit.myPlayer.id == player && 
					(!withActions || unit.myStats.ActionPoints > 0)) {
					return unit;
				}
				currentIndex = (currentIndex + 1) % units.Count;
			}
		}

		return null;
	}

	public UnitController GetPreviousUnit (int player, bool withActions = true) {
		if (selectedUnit == null) {
			return GetFirstUnit (player, withActions);
		} else {
			int startingIndex = units.FindIndex ((unit) => unit == selectedUnit);
			int currentIndex = startingIndex - 1;

			if (currentIndex < 0) {
				currentIndex = units.Count - 1;
			}

			while (currentIndex != startingIndex) {
				UnitController unit = units [currentIndex];
				if (unit.myPlayer.id == player && 
					(!withActions || unit.myStats.ActionPoints > 0)) {
					return unit;
				}

				currentIndex--;
				if (currentIndex < 0) {
					currentIndex = units.Count - 1;
				}
			}
		}

		return null;
	}

	public UnitController GetFirstUnit(int player, bool withActions = true) {
		return units.Find ((unit) => {
			bool belongsToPlayer = unit.myPlayer.id == player;
			bool hasActions = !withActions || unit.myStats.ActionPoints > 0;
			return belongsToPlayer && hasActions;
		});
	}
}
