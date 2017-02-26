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
		SpawnUnit (0, players[0], 5, 5);
		SpawnUnit (1, players[1], 4, 4);
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

	public Dictionary<Node, float> FindReachableTiles(UnitController unit) {
		return myMap.pathfinder.findReachableTiles (unit.myNode, unit.myStats.Speed, unit.myStats.WalkingType, unit.myPlayer.faction);
	}

	public void ShowMovement(UnitController unit) {
		Dictionary<Node, float> reachableTiles = FindReachableTiles(unit);
		myMap.highlighter.UnhighlightTiles ();
		myMap.highlighter.HighlightTile (unit.myNode, SquareTarget.NONE);
		myMap.highlighter.HighlightTiles (reachableTiles.Keys.ToList(), SquareTarget.MOVEMENT);
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
		List<Node> attackableTiles = myMap.pathfinder.FindAttackableTiles (selectedUnit, activeAbility);
		myMap.highlighter.UnhighlightTiles ();
		myMap.highlighter.HighlightTile (selectedUnit.myNode, SquareTarget.NONE);
		myMap.highlighter.HighlightTiles (attackableTiles, SquareTarget.ATTACK);
		return true;

	}

	public bool AttackTile(Node targetNode) {
		if (!activeAbility.CanTargetTile (selectedUnit, targetNode)) {
			return false;
		}

		Vector2 direction = myMap.GetDirectionBetweenNodes (selectedUnit.myNode, targetNode);
		activeAbility.UseAbility (selectedUnit, targetNode, direction);
		selectedUnit.FaceDirection (direction);
		selectedUnit.SetAttacking (true);
		selectedUnit.myStats.ActionPoints--;

		return true;
	}

	public void MoveToTile(Node targetNode) {
		MovementPath path = myMap.pathfinder.getPathFromTile (targetNode);
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

	public bool PlayerOutOfActions(int playerId) {
		bool noActionsRemaining = true;

		foreach (UnitController unit in units) {
			if (unit.myPlayer.id == playerId && (unit.myStats.ActionPoints > 0 || !unit.myStats.HasMoved)) {
				noActionsRemaining = false;
			}
		}

		return noActionsRemaining;

	}
}
