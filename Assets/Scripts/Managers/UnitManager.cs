using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

public class UnitManager : NetworkBehaviour {

    public static UnitManager singleton;

	List<UnitController> units = new List<UnitController>();
	List<UnitController> unitsToRemove = new List<UnitController>();

	public GameObject[] unitPrefabs;

	public TileMap myMap;

	GUIController guiController;

	//currentSelect
	UnitController currentPlayerUnit = null;
    AttackAction activeAbility = null;
	List<Node> attackableTiles = new List<Node>();

	Node currentlyHoveredNode;

    private void Awake() {
        singleton = this;
    }

	public void Initialise(TileMap map) {
		//guiController = GetComponentInChildren<GUIController> ();
		myMap = map;
	}

	public List<UnitController> Units {
		get { return units; }
	}

    public GameObject SpawnUnit(int unitId, int x, int y, int playerId) {
        Node startingNode = myMap.getNode(x, y);
        GameObject newUnit = Instantiate(unitPrefabs[unitId], myMap.getPositionOfNode(startingNode), Quaternion.identity);

        UnitController unitController = newUnit.GetComponent<UnitController>();

        startingNode.SetMyUnit(unitController);
        unitController.myNode = startingNode;
        unitController.FaceDirection(Vector2.down);
        unitController.myPlayerId = playerId;
        units.Add(unitController);

        return newUnit;
    }

    public void StartTurn(int playersTurn) {
        currentPlayerUnit = units.Find(unit => unit.myPlayerId == playersTurn);
        //RemoveUnits();
    }

    public void EndTurn(int playersTurn) {
        //currentPlayerUnit.EndTurn();
    }

    // COMMANDs
    /////////////////////////////



    // SERVERs
    /////////////////////////////



    // LEGACY
    /////////////////////////////

    public Node CurrentlyHoveredNode {
        get { return currentlyHoveredNode; }
        set { currentlyHoveredNode = value; }
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



    public void CardPlayed(AbilityCardBase card) {
        
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

    public void ShowMoveAction(int moveDistance, Walkable walkingType) {
        if (currentPlayerUnit == null) {
            throw new System.Exception("Current player not selected!");
        }

        PlayerConnectionObject player = GameManager.singleton.players[currentPlayerUnit.myPlayerId];

        ReachableTiles walkingTiles = myMap.pathfinder.findReachableTiles(currentPlayerUnit.myNode, moveDistance, walkingType, player.faction);
        myMap.highlighter.HighlightTiles(walkingTiles.basic.Keys.ToList(), SquareTarget.MOVEMENT);
    }

    public void ClearMovementTiles() {
        myMap.highlighter.UnhighlightTiles();
    }

    // Shows where the ability can target
    public bool ShowAttackAction(AttackAction action)
    {

        if (currentPlayerUnit == null)
        {
            guiController.ShowErrorMessage("Player doesn't have a character?!");
            return false;
        }

        //if (currentPlayerUnit.myStats.ActionPoints <= 0)
        //{
        //    guiController.ShowErrorMessage("Not enough action points");
        //    return false;
        //}

        action.caster = currentPlayerUnit;
        activeAbility = action;

        attackableTiles = myMap.pathfinder.FindAttackableTiles(currentPlayerUnit.myNode, action);
        myMap.highlighter.UnhighlightAllTiles();
        myMap.highlighter.HighlightTile(currentPlayerUnit.myNode, SquareTarget.NONE);
        SquareTarget targetType = action.targets == TargetType.ALLY ? SquareTarget.HELPFULL : SquareTarget.ATTACK;
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
	public bool AttackTile(Node targetNode, AttackAction attackAction = null) {

		if (attackAction == null) {
            attackAction = activeAbility;
		}

        if (attackAction != null && !attackAction.CanTargetTile(targetNode)) {
            return false;
        }

  //      if (targetNode.previousMoveNode) {
  //          //MovementPath movementPath = myMap.pathfinder.getPathFromTile (targetNode.previousMoveNode);
		//	MovementPath movementPath = myMap.pathfinder.FindPath (currentPlayerUnit.myNode, targetNode.previousMoveNode, currentPlayerUnit.myStats.WalkingType, currentPlayerUnit.myPlayer.faction);
		//	SetUnitPath (movementPath);
		//}

		//Action action = new Action();
  //      action.type = ActionType.ATTACK;
  //      action.ability = attackAction;
  //      action.nodes = GetTargetNodes(attackAction, targetNode);

  //      currentPlayerUnit.AddAction (action);

        return true;
	}

    // Shows the attackable tiles of the ability
	List<Node> GetTargetNodes(AttackAction action, Node targetNode) {
		List<Node> targetTiles = new List<Node> ();
		switch (action.areaOfEffect) {
		case AreaOfEffect.AURA:
			return attackableTiles;
		case AreaOfEffect.CIRCLE:
			return myMap.pathfinder.FindAOEHitTiles(targetNode, action);
		case AreaOfEffect.CLEAVE:
			return myMap.pathfinder.FindCleaveTargetTiles(targetNode, action, currentPlayerUnit.myNode);
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
        currentPlayerUnit.AddAction (moveAction);
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
