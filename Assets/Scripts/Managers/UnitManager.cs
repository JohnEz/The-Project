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
	UnitController currentPlayerUnit = null;
    AbilityCardBase activeAbility = null;
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

        // Load from static file for these?
        SpawnUnit(7, players[0], 8, 10);

		SpawnUnit (2, players[1], 17, 10);
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
		foreach (UnitController unit in units) {
			if (unit.myPlayer.id == playersTurn) {
				unit.EndTurn ();
			}
		}
	}

	public ReachableTiles FindReachableTiles(UnitController unit) {
		return myMap.pathfinder.findReachableTiles (unit.myNode, unit.myStats.Speed, unit.myStats.WalkingType, unit.myPlayer.faction);
	}

    // TODO this is probably needed for advanced cards but not for basic, fix later
    // shows movement and attack tiles
    //public MovementAndAttackPath ShowActions(UnitController unit = null) {
    //	unit = unit == null ? selectedUnit : unit;

    //	myMap.highlighter.UnhighlightAllTiles ();
    //	myMap.highlighter.HighlightTile (unit.myNode, SquareTarget.NONE);

    //	UnitClass unitClass = unit.GetComponent<UnitClass>();
    //	MovementAndAttackPath reachableTiles = myMap.pathfinder.findMovementAndAttackTiles (unit, unitClass.abilities [0], unit.myStats.ActionPoints);
    //	activeAbility = unitClass.abilities[0];

    //	myMap.highlighter.HighlightTiles (reachableTiles.movementTiles.basic.Keys.ToList(), SquareTarget.MOVEMENT);
    //	myMap.highlighter.HighlightTiles (reachableTiles.movementTiles.extended.Keys.ToList(), SquareTarget.DASH);
    //	myMap.highlighter.HighlightTiles (reachableTiles.attackTiles, SquareTarget.ATTACK);

    //	return reachableTiles;
    //}

    // Shows where the ability can target
    public bool ShowAbility(AbilityCardBase ability)
    {

        if (selectedUnit == null)
        {
            guiController.ShowErrorMessage("No unit selected");
            return false;
        }

        if (selectedUnit.myStats.ActionPoints <= 0)
        {
            guiController.ShowErrorMessage("Not enough action points");
            return false;
        }

        // TODO this probabily will need to change
        activeAbility = ability;

        attackableTiles = myMap.pathfinder.FindAttackableTiles(selectedUnit.myNode, activeAbility);
        myMap.highlighter.UnhighlightAllTiles();
        myMap.highlighter.HighlightTile(selectedUnit.myNode, SquareTarget.NONE);
        SquareTarget targetType = activeAbility.targets == TargetType.ALLY ? SquareTarget.HELPFULL : SquareTarget.ATTACK;
        myMap.highlighter.HighlightTiles(attackableTiles, targetType);

        if (currentlyHoveredNode != null)
        {
            HighlightEffectedTiles(currentlyHoveredNode);
        }

        return true;
    }

    // Highlight the tiles effected by the ability
    public void HighlightEffectedTiles(Node target) {
		if (attackableTiles.Contains (target)) {
			List<Node> effectedNodes = GetTargetNodes (activeAbility, target);
			myMap.highlighter.ShowAbilityTiles (effectedNodes, activeAbility);
		}
	}

    // TODO i dont know why this is its own fucntion call?
    // unhighlight all tiles
	public void UnhiglightEffectedTiles() {
		myMap.highlighter.ClearEffectedTiles ();
	}

    // shows the path to the selected node
	public void ShowPath(Node targetNode) {
		MovementPath movementPath = myMap.pathfinder.getPathFromTile (targetNode);
		myMap.highlighter.ShowPath (movementPath.path);
	}

    // Uses the specified ability of at the target location
	public bool AttackTile(Node targetNode, AbilityCardBase ability = null) {

		if (ability == null) {
			ability = activeAbility;
		}

		if (ability != null && !ability.CanTargetTile (targetNode)) {
			return false;
		}

		if (targetNode.previousMoveNode) {
			//MovementPath movementPath = myMap.pathfinder.getPathFromTile (targetNode.previousMoveNode);
			UnitController currentUnit = selectedUnit != null ? selectedUnit : ability.caster;
			MovementPath movementPath = myMap.pathfinder.FindPath (currentUnit.myNode, targetNode.previousMoveNode, currentUnit.myStats.WalkingType, currentUnit.myPlayer.faction);
			SetUnitPath (movementPath);
		}

		Action attackAction = new Action();
		attackAction.type = ActionType.ATTACK;
		attackAction.ability = ability;
		attackAction.nodes = GetTargetNodes(ability, targetNode);

		ability.caster.AddAction (attackAction);

		return true;
	}

    // Shows the attackable tiles of the ability
	List<Node> GetTargetNodes(AbilityCardBase ability, Node targetNode) {
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

    // Adds a move action to a units queue
	public void SetUnitPath(MovementPath movementPath) {
		Action moveAction = new Action();
		moveAction.type = ActionType.MOVEMENT;
		moveAction.nodes = movementPath.path;
		selectedUnit.AddAction (moveAction);
	}

    // Tells the unit to move to a set node
	public void MoveToTile(Node targetNode) {
		MovementPath movementPath = myMap.pathfinder.getPathFromTile (targetNode);
		SetUnitPath(movementPath);
	}

    // Called when the unit starts following a path
	public void UnitStartedMoving() {
		TurnManager turnManager = GetComponent<TurnManager> ();
		turnManager.StartMoving ();
	}

    // Called when a unit has reached the end of its path
	public void UnitFinishedMoving() {
		TurnManager turnManager = GetComponent<TurnManager> ();
		turnManager.FinishedMoving ();
	}

    // Called at the start of a unit attack
	public void UnitStartedAttacking() {
		TurnManager turnManager = GetComponent<TurnManager> ();
		turnManager.StartAttacking ();
	}

    // Called when a unit finishes its attack 
	public void UnitFinishedAttacking() {
		TurnManager turnManager = GetComponent<TurnManager> ();
		
		RemoveUnits ();

        //TODO This seems odd, or at least should be a constant value
        StartCoroutine(CallActionWithDelay(() => turnManager.FinishedAttacking(), 0.2f));
	}

    IEnumerator CallActionWithDelay(System.Action action, float seconds) {
        yield return new WaitForSeconds(seconds);
        action();
    }

    // Called when a unit dies, removes them from the game
	public void UnitDied(UnitController unit) {
		AddUnitToRemove (unit);
	}

    // Check to see if a specified player has run out of moves
	public bool PlayerOutOfActions(int playerId) {
		return false;
	}
}
