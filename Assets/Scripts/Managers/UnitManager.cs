using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class UnitManager : MonoBehaviour {
    public static UnitManager instance;

    public GameObject UnitPrefab;
    public List<UnitController> Units { get; private set; }
    private List<UnitController> unitsToRemove;

    private AttackAction activeAbility = null;

    private List<Node> attackableTiles = new List<Node>();

    private Node currentlyHoveredNode;

    [Serializable] public class OnUnitDieEvent : UnityEvent<UnitController> { }

    public OnUnitDieEvent onUnitDie = new OnUnitDieEvent();

    private void Awake() {
        instance = this;
    }

    public Node CurrentlyHoveredNode {
        get { return currentlyHoveredNode; }
        set { currentlyHoveredNode = value; }
    }

    // Use this for initialization
    private void Start() {
    }

    public void Initialise() {
        Units = new List<UnitController>();
        unitsToRemove = new List<UnitController>();
    }

    // Update is called once per frame
    private void Update() {
    }

    public List<UnitController> GetPlayersUnits(int playerId) {
        return Units.Where(unit => unit.myPlayer.id == playerId).ToList();
    }

    public UnitController SpawnUnit(string unit, Player player, int x, int y) {
        if (!ResourceManager.instance.units.ContainsKey(unit)) {
            Debug.LogError("Could not spawn character " + unit);
            return null;
        }
        UnitObject unitToSpawn = ResourceManager.instance.units[unit];

        return SpawnUnit(Instantiate(unitToSpawn), player, x, y);
    }

    public UnitController SpawnUnit(UnitObject unit, Player player, int x, int y, Vector2? dir = null) {
        if (dir == null) {
            dir = Vector2.left;
        }
        // TODO 3d Refactor, unit spawn locations should only do X and Z
        GameObject newUnit = (GameObject)Instantiate(UnitPrefab, TileMap.instance.getPositionOfNode(x, y), Quaternion.identity);
        newUnit.transform.parent = TileMap.instance.transform;

        Node startingNode = TileMap.instance.GetNode(x, y);
        UnitController unitController = newUnit.GetComponent<UnitController>();

        startingNode.myUnit = unitController;
        unitController.Spawn(player, startingNode, unit);
        unitController.FaceDirection(Vector2.down);
        unitController.myManager = this;
        unitController.Initialise();
        Units.Add(unitController);

        player.units.Add(unitController);

        return unitController;
    }

    public void AddUnitToRemove(UnitController unit) {
        unit.myNode.myUnit = null;
        unitsToRemove.Add(unit);
    }

    public void RemoveUnits() {
        unitsToRemove.ForEach((unit) => {
            Units.Remove(unit);
        });

        unitsToRemove.Clear();
    }

    public void StartTurn(Player player) {
        foreach (UnitController unit in Units) {
            if (unit.myPlayer.id == player.id) {
                // TODO sort player selection as this is a bad hack
                //currentPlayerUnit = player.myCharacter;
                unit.NewTurn();
            }
        }
        RemoveUnits();
    }

    public void EndTurn(Player player) {
        foreach (UnitController unit in Units) {
            if (unit.myPlayer.id == player.id) {
                unit.EndTurn();
            }
        }
    }

    public void CardPlayed(Ability card) {
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

    public void ShowMoveAction(UnitController unit, int moveDistance, Walkable walkingType) {
        if (unit == null) {
            throw new System.Exception("Current player not selected!");
        }

        ReachableTiles walkingTiles = TileMap.instance.pathfinder.findReachableTiles(unit.myNode, moveDistance, walkingType, new PathSearchOptions(unit.myPlayer.faction));
        HighlightManager.instance.HighlightTiles(walkingTiles.basic.Keys.ToList(), SquareTarget.MOVEMENT);
        HighlightManager.instance.HighlightTile(unit.myNode, SquareTarget.NONE);
    }

    // Shows where the ability can target
    public bool ShowAttackAction(UnitController unit, AttackAction action) {
        if (unit == null) {
            GUIController.instance.ShowErrorMessage("Player doesn't have a character?!");
            return false;
        }

        //if (currentPlayerUnit.myStats.ActionPoints <= 0)
        //{
        //    guiController.ShowErrorMessage("Not enough action points");
        //    return false;
        //}

        action.caster = unit;
        activeAbility = action;

        attackableTiles = TileMap.instance.pathfinder.FindAttackableTiles(unit.myNode, action);
        HighlightManager.instance.UnhighlightAllTiles();
        HighlightManager.instance.HighlightTile(unit.myNode, SquareTarget.NONE);
        SquareTarget targetType = action.targets == TargetType.ALLY ? SquareTarget.HELPFULL : SquareTarget.ATTACK;
        HighlightManager.instance.HighlightTiles(attackableTiles, targetType);

        if (currentlyHoveredNode != null) {
            HighlightEffectedTiles(unit, currentlyHoveredNode);
        }

        return true;
    }

    // Highlight the tiles effected by the ability
    public void HighlightEffectedTiles(UnitController unit, Node target) {
        if (attackableTiles.Contains(target)) {
            List<Node> effectedNodes = TileMap.instance.pathfinder.FindEffectedTiles(unit.myNode, target, activeAbility);
            HighlightManager.instance.ShowAbilityTiles(effectedNodes, activeAbility);
        }
    }

    // TODO i dont know why this is its own fucntion call?
    // unhighlight all tiles
    public void UnhiglightEffectedTiles() {
        HighlightManager.instance.ClearEffectedTiles();
    }

    // shows the path to the selected node
    public void ShowPath(Node targetNode) {
        MovementPath movementPath = TileMap.instance.pathfinder.getPathFromTile(targetNode);
        HighlightManager.instance.ShowPath(movementPath.path);
    }

    // Uses the specified ability of at the target location
    public bool AttackTile(UnitController unit, Node targetNode, AttackAction attackAction = null) {
        if (attackAction == null) {
            attackAction = activeAbility;
        }

        if (attackAction != null && !attackAction.CanTargetTile(targetNode)) {
            return false;
        }

        if (targetNode.previousMoveNode) {
            //MovementPath movementPath = myMap.pathfinder.getPathFromTile (targetNode.previousMoveNode);
            MovementPath movementPath = TileMap.instance.pathfinder.FindPath(unit.myNode, targetNode.previousMoveNode, unit.myStats.WalkingType, new PathSearchOptions(unit.myPlayer.faction));
            SetUnitPath(unit, movementPath);
        }

        Action action = new Action();
        action.type = ActionType.ATTACK;
        action.ability = attackAction;
        action.nodes = TileMap.instance.pathfinder.FindEffectedTiles(unit.myNode, targetNode, attackAction);

        unit.AddAction(action);

        return true;
    }

    // Adds a move action to a units queue
    public void SetUnitPath(UnitController unit, MovementPath movementPath) {
        Action moveAction = new Action();
        moveAction.type = ActionType.MOVEMENT;
        moveAction.nodes = movementPath.path;
        unit.AddAction(moveAction);
    }

    // Tells the unit to move to a set node
    public void MoveToTile(UnitController unit, Node targetNode) {
        MovementPath movementPath = TileMap.instance.pathfinder.getPathFromTile(targetNode);
        SetUnitPath(unit, movementPath);
    }

    // Called when the unit starts following a path
    public void UnitStartedMoving() {
        TurnManager.instance.StartMoving();
    }

    // Called when a unit has reached the end of its path
    public void UnitFinishedMoving(UnitController unit) {
        TurnManager.instance.FinishedMoving();
    }

    // Called at the start of a unit attack
    public void UnitStartedAttacking() {
        TurnManager.instance.StartAttacking();
    }

    // Called when a unit finishes its attack
    public void UnitFinishedAttacking() {
        RemoveUnits();

        //TODO this is done to not jump around, should at least be a constant value
        StartCoroutine(CallActionWithDelay(() => TurnManager.instance.FinishedAttacking(), 0.2f));
    }

    private IEnumerator CallActionWithDelay(System.Action action, float seconds) {
        yield return new WaitForSeconds(seconds);
        action();
    }

    // Called when a unit dies, removes them from the game
    public void UnitDied(UnitController unit) {
        AddUnitToRemove(unit);

        if (onUnitDie != null) {
            onUnitDie.Invoke(unit);
        }
    }

    // Check to see if a specified player has run out of moves
    public bool PlayerOutOfActions(int playerId) {
        return false;
    }
}