using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UnitManager : MonoBehaviour {

	List<UnitController> units;
	List<UnitController> unitsToRemove;

	public GameObject[] unitPrefabs;

	TileMap myMap;

	GUIController guiController;

	//currentSelect
	UnitController selectedUnit = null;
	public UnitController lastSelectedUnit = null;
	BaseAbility activeAbility = null;
	List<Node> attackableTiles = new List<Node>();

	Node currentlyHoveredNode;

	public Node CurrentlyHoveredNode {
		get { return currentlyHoveredNode; }
		set { currentlyHoveredNode = value; }
	}

	// Use this for initialization
	void Start () {

	}

	public void Initialise(List<Player> players, TileMap map) {
		guiController = GetComponentInChildren<GUIController> ();
		units = new List<UnitController> ();
		unitsToRemove = new List<UnitController> ();
		myMap = map;
		SpawnUnit (5, players[0], 3, 8);
		SpawnUnit (3, players[0], 3, 9);
		SpawnUnit (4, players[0], 3, 10);
		SpawnUnit (0, players[1], 9, 9);
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

	public void AddUnitToRemove(UnitController unit) {
		unit.myNode.myUnit = null;
		unitsToRemove.Add (unit);
	}

	public void RemoveUnits() {
		unitsToRemove.ForEach ((unit) => {
			units.Remove(unit);
		});

		unitsToRemove.Clear ();
	}

	public void StartTurn(int playersTurn) {
		foreach (UnitController unit in units) {
			if (unit.myPlayer.id == playersTurn) {
				unit.NewTurn ();
			}
		}
		RemoveUnits ();
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

	public UnitController SelectedUnit {
		get { return selectedUnit; }
	}

	public ReachableTiles FindReachableTiles(UnitController unit) {
		return myMap.pathfinder.findReachableTiles (unit.myNode, unit.myStats.Speed, unit.myStats.WalkingType, unit.myPlayer.faction);
	}

	// shows movement and attack tiles
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
			guiController.ShowErrorMessage ("No unit selected");
			return false;
		}

		if (selectedUnit.myStats.ActionPoints <= 0) {
			guiController.ShowErrorMessage ("Not enough action points");
			return false;
		}

		unitClass = selectedUnit.GetComponent<UnitClass> ();

		if (!unitClass.HasAbility (ability)) {
			guiController.ShowErrorMessage ("ERROR: NO ABILITY AT INDEX " + ability);
			return false;
		}

		activeAbility = unitClass.abilities [ability];

		if (activeAbility.IsOnCooldown ()) {
			guiController.ShowErrorMessage ("That ability is still on cooldown");
			return false;
		}

		
		attackableTiles = myMap.pathfinder.FindAttackableTiles (selectedUnit.myNode, activeAbility);
		myMap.highlighter.UnhighlightAllTiles ();
		myMap.highlighter.HighlightTile (selectedUnit.myNode, SquareTarget.NONE);
		SquareTarget targetType = activeAbility.targets == TargetType.ALLY ? SquareTarget.HELPFULL : SquareTarget.ATTACK;
		myMap.highlighter.HighlightTiles (attackableTiles, targetType);

		if (currentlyHoveredNode != null) {
			HighlightEffectedTiles (currentlyHoveredNode);
		}

		return true;
	}

	public void HighlightEffectedTiles(Node target) {
		if (attackableTiles.Contains (target)) {
			List<Node> effectedNodes = GetTargetNodes (activeAbility, target);
			myMap.highlighter.ShowAbilityTiles (effectedNodes, activeAbility);
		}
	}

	public void UnhiglightEffectedTiles() {
		myMap.highlighter.ClearEffectedTiles ();
	}

	public void ShowPath(Node targetNode) {
		MovementPath movementPath = myMap.pathfinder.getPathFromTile (targetNode);
		myMap.highlighter.ShowPath (movementPath.path);
	}

	public bool AttackTile(Node targetNode) {
		if (!activeAbility.CanTargetTile (targetNode)) {
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
		case AreaOfEffect.CLEAVE:
			return myMap.pathfinder.FindCleaveTargetTiles(targetNode, ability, selectedUnit.myNode);
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
		RemoveUnits ();
	}

	public void UnitDied(UnitController unit) {
		AddUnitToRemove (unit);
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
