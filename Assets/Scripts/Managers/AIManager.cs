using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AITurnPlan {
    public Node targetMoveNode = null;
    public AIAttackAction attack = null;
    public int valueOfPlan = 0;
}

public class AIAttackAction {
    public AttackAction attack = null;
    public Node targetNode = null;
    public int valueOfAttack = 0;

    public AIAttackAction(AttackAction _attack, Node _targetNode, int _valueOfAttack = 0) {
        attack = _attack;
        targetNode = _targetNode;
        valueOfAttack = _valueOfAttack;
    }
}

public class AIManager : MonoBehaviour {
    public static AIManager instance;

    private List<UnitController> myUnits;

    private void Awake() {
        instance = this;
    }

    // Use this for initialization
    private void Start() {
    }

    // Update is called once per frame
    private void Update() {
    }

    public List<UnitController> MyUnits {
        get { return myUnits; }
        set { myUnits = value; }
    }

    public void Initialise(int myPlayerId) {
        myUnits = UnitManager.instance.GetPlayersUnits(myPlayerId);

        foreach (UnitController unit in myUnits) {
            if (unit) {
                DisplayNextTurn(unit);
            }
        }
    }

    /// Turns
    ///////////////////

    // NewTurn is called at the start of each of the AIs turns.
    public IEnumerator NewTurn(int myPlayerId) {
        myUnits = UnitManager.instance.GetPlayersUnits(myPlayerId);

        foreach (UnitController unit in myUnits) {
            // TODO monster check
            if (unit) {
                CameraManager.instance.FollowTarget(unit.transform);
                yield return new WaitForSeconds(CameraManager.instance.blendTime);
                yield return TurnManager.instance.WaitForWaitingForInput();
                unit.unitCanvasController.HideTelegraph();
                yield return RunAI(unit);
                DisplayNextTurn(unit);
            }
        }

        // TODO this shouldnt be needed if we skip ai turns with no active units
        yield return TurnManager.instance.WaitForWaitingForInput();

        TurnManager.instance.EndTurn();
    }

    public IEnumerator RunAI(UnitController unit) {
        Monster monster = (Monster)unit.myStats;

        MonsterAI monsterTurn = monster.GetNextTurn();

        monster.currentTarget = FindTarget(unit, monsterTurn);

        // If it cant find a suitable target, do default turn instead
        if (monster.currentTarget == null && !(monsterTurn.targetPriority.Contains(MonsterTarget.NONE) || monsterTurn.targetPriority.Contains(MonsterTarget.ALL))) {
            // TODO this should not require a target
            monsterTurn = monster.defaultTurn;
            monster.currentTarget = FindTarget(unit, monsterTurn);
        }

        unit.CreateBasicText(monsterTurn.turnName);

        foreach (MonsterAction monsterAction in monsterTurn.actions) {
            yield return RunMonsterAction(unit, monsterAction, monster.currentTarget);
        }

        monster.currentTarget = null;
        yield return TurnManager.instance.WaitForWaitingForInput();

        if (monsterTurn.takeAnotherTurn) {
            yield return RunAI(unit);
        }
    }

    public IEnumerator RunMonsterAction(UnitController unit, MonsterAction action, UnitController target) {
        switch (action.ActionType) {
            case MonsterActionType.MOVE:
                yield return Move(unit, action, target);
                break;

            case MonsterActionType.ATTACK:
                yield return Attack(unit, (MonsterAttackAction)action, target);
                break;
        }

        yield return TurnManager.instance.WaitForWaitingForInput();
    }

    private void DisplayNextTurn(UnitController unit) {
        Monster monster = (Monster)unit.myStats;
        MonsterAI monsterTurn = monster.PeekNextTurn();

        unit.unitCanvasController.ShowTelegraph(monsterTurn.telegraph);
    }

    /// Target Picking
    ///////////////////

    private UnitController FindTarget(UnitController unit, MonsterAI turn) {
        UnitController target = null;
        Tile blindSpot = GetBlindSpot(unit);

        turn.targetPriority.ForEach((MonsterTarget targetType) => {
            if (target != null) {
                return;
            }
            switch (targetType) {
                case MonsterTarget.FOCUSED:
                    target = FindFocusedTarget(unit, blindSpot, turn.ignoreBlindSpot);
                    break;

                case MonsterTarget.LAST_INJURY:
                    target = FindLastInjuryTarget(unit, blindSpot, turn.ignoreBlindSpot);
                    break;

                case MonsterTarget.FACING:
                    target = FindFacingTarget(unit, blindSpot, turn.ignoreBlindSpot);
                    break;

                case MonsterTarget.CLOSEST:
                    target = FindClosestTarget(unit, UnitManager.instance.Units, blindSpot, turn.ignoreBlindSpot);
                    break;

                case MonsterTarget.ALL:
                    target = unit;
                    break;
            }
        });

        return target;
    }

    // TODO these two methods can become one
    private UnitController FindFocusedTarget(UnitController unit, Tile blindSpot, bool ignoreBlindSpot) {
        Monster monster = (Monster)unit.myStats;
        if (monster.focusedTarget) {
            bool canSee = ignoreBlindSpot || !blindSpot.Contains(monster.focusedTarget.myTile);
            if (canSee) {
                return monster.focusedTarget;
            }
        }
        return null;
    }

    private UnitController FindLastInjuryTarget(UnitController unit, Tile blindSpot, bool ignoreBlindSpot) {
        Monster monster = (Monster)unit.myStats;
        if (monster.lastInjuryTarget) {
            bool canSee = ignoreBlindSpot || !blindSpot.Contains(monster.lastInjuryTarget.myTile);
            if (canSee) {
                return monster.lastInjuryTarget;
            }
        }
        return null;
    }

    private UnitController FindFacingTarget(UnitController unit, Tile blindSpot, bool ignoreBlindSpot) {
        Dictionary<UnitController, float> unitToDistance = new Dictionary<UnitController, float>();
        UnitManager.instance.Units.ForEach(otherUnit => {
            if (otherUnit.myPlayer.faction != unit.myPlayer.faction) {
                //TODO add LoS check
                bool canSee = ignoreBlindSpot || !blindSpot.Contains(otherUnit.myTile);
                if (unit.facingDirection == unit.GetDirectionToTile(otherUnit.myTile) && canSee) {
                    //TODO change this to move distance
                    Vector2 my2DPosition = new Vector2(unit.myTile.X, unit.myTile.Y);
                    Vector2 target2DPosition = new Vector2(otherUnit.myTile.X, otherUnit.myTile.Y);
                    float distanceToTarget = Vector2.Distance(my2DPosition, target2DPosition);

                    unitToDistance.Add(otherUnit, distanceToTarget);
                }
            }
        });

        UnitController closestFacedUnit = null;
        if (unitToDistance.Count > 0) {
            closestFacedUnit = unitToDistance.First().Key;
            unitToDistance.Keys.ToList().ForEach((otherUnit) => {
                if (unitToDistance[otherUnit] < unitToDistance[closestFacedUnit]) {
                    closestFacedUnit = otherUnit;
                }
            });
        }

        return closestFacedUnit;
    }

    private UnitController FindClosestTarget(UnitController unit, List<UnitController> targets, Tile blindSpot, bool ignoreBlindSpot) {
        int shortestPath = -1;
        UnitController closestTarget = null;
        targets.ForEach(otherUnit => {
            if (otherUnit.myPlayer.faction != unit.myPlayer.faction) {
                bool canSee = ignoreBlindSpot || !blindSpot.Contains(otherUnit.myTile);
                if (!canSee) {
                    return;
                }

                MovementPath pathToTarget = TileMap.instance.pathfinder.FindShortestPathToUnit(unit.myTile, otherUnit.myTile, unit.myStats.walkingType, new PathSearchOptions(unit.myPlayer.faction, unit.myStats.size));

                if (pathToTarget.movementCost > -1) {
                    if (pathToTarget.movementCost < shortestPath || shortestPath == -1) {
                        shortestPath = pathToTarget.movementCost;
                        closestTarget = otherUnit;
                    }
                }
            }
        });

        return closestTarget;
    }

    public Tile GetBlindSpot(UnitController unit) {
        int x = unit.myTile.x - (int)unit.facingDirection.x;
        int y = unit.myTile.y + (int)unit.facingDirection.y;
        return TileMap.instance.GetTile(unit.myStats.size, x, y);
    }

    /// Movement
    ///////////////////

    public IEnumerator Move(UnitController unit, MonsterAction action, UnitController target) {
        MovementPath pathToTarget = new MovementPath();
        pathToTarget.movementCost = -1;

        switch (action.MoveType) {
            case MonsterMoveType.TOWARDS_TARGET:
                //Debug.Log("Finding path to target");
                pathToTarget = TileMap.instance.pathfinder.FindShortestPathToUnit(unit.myTile, target.myTile, unit.myStats.walkingType, new PathSearchOptions(unit.myPlayer.faction, unit.myStats.size));
                //pathToTarget = TileMap.instance.pathfinder.FindPath(unit.myTile, shortestDestination, unit.myStats.walkingType, new PathSearchOptions(unit.myPlayer.faction, unit.myStats.size));
                break;

            case MonsterMoveType.AWAY_FROM_TARGET:
                Debug.LogError("Move away from target not yet implemented");
                break;
        }

        // Check there is a path to that node
        if (pathToTarget.movementCost > -1) {
            pathToTarget = RemoveTilesWithUnits(unit, pathToTarget);
            pathToTarget.path = Pathfinder.CleanPath(pathToTarget.path, unit.myTile);

            // Tell unit to follow path
            if (pathToTarget.path.Count > 0) {
                CameraManager.instance.FollowTarget(unit.transform);
                unit.AddMoveAction(pathToTarget);
            }
        } else {
            Debug.LogError(String.Format("Unit \"{0}\" cant move to node {1}", unit.name, target.myTile));
        }

        yield return TurnManager.instance.WaitForWaitingForInput();
    }

    private MovementPath RemoveTilesWithUnits(UnitController unit, MovementPath movementPath) {
        if (movementPath.movementCost == 0) {
            return movementPath;
        }

        movementPath.path = movementPath.path.Take(unit.myStats.Speed).ToList();

        //Debug.Log("I want to move to node: " + shortestPath.path.Last());

        // if there is a unit on the final node
        while (movementPath.path.Count > 0 && movementPath.path.Last().ContainsAUnitExcluding(unit)) {
            //remove that node
            movementPath.path.Remove(movementPath.path.Last());
        }

        return movementPath;
    }

    /// Attacking
    ///////////////////

    public IEnumerator Attack(UnitController unit, MonsterAttackAction action, UnitController target) {
        if (target != unit) {
            CameraManager.instance.FollowTarget(target.transform);
            yield return new WaitForSeconds(CameraManager.instance.blendTime);
        } else {
            CameraManager.instance.ZoomOutCamera(target.transform);
            yield return new WaitForSeconds(CameraManager.instance.blendTime);
        }

        AttackTile(unit, target.myTile.Nodes.First(), action.Attack);

        yield return TurnManager.instance.WaitForWaitingForInput();
    }

    public void AttackTile(UnitController unit, Node targetTile, AttackAction attack) {
        // gets the first target of the first ability
        //Debug.Log("AI - attacking tile: " + targetTile);
        if (attack.range > -1 && attack.range > targetTile.GridDistanceTo(unit.myTile)) {
            return;
        }

        UnitManager.instance.AttackTile(unit, targetTile, attack);
    }
}