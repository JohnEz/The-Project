﻿using System;
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

    // NewTurn is called at the start of each of the AIs turns.
    public IEnumerator NewTurn(int myPlayerId) {
        myUnits = UnitManager.instance.GetPlayersUnits(myPlayerId);

        foreach (UnitController unit in myUnits) {
            // TODO monster check
            if (unit) {
                CameraManager.instance.FollowTarget(unit.transform);
                yield return new WaitForSeconds(CameraManager.instance.blendTime);
                yield return TurnManager.instance.WaitForWaitingForInput();
                //yield return PlanTurnTwoActions(unit);
                yield return RunAI(unit);
            }
        }

        // TODO this shouldnt be needed if we skip ai turns with no active units
        yield return TurnManager.instance.WaitForWaitingForInput();

        TurnManager.instance.EndTurn();
    }

    public IEnumerator RunAI(UnitController unit) {
        Monster monster = (Monster)unit.myStats;

        int turnIndex = UnityEngine.Random.Range(0, monster.myTurns.Count);
        MonsterAI monsterTurn = monster.myTurns[turnIndex];

        monster.currentTarget = FindTarget(unit, monster, monsterTurn);

        // If it cant find a suitable target, do default turn instead
        if (monster.currentTarget == null && !monsterTurn.targetPriority.Contains(MonsterTarget.NONE)) {
            monsterTurn = monster.defaultTurn;
        }

        foreach (MonsterAction monsterAction in monsterTurn.actions) {
            yield return RunMonsterAction(unit, monsterAction, monster.currentTarget);
        }

        monster.currentTarget = null;
        yield return TurnManager.instance.WaitForWaitingForInput();
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

    /// Target Picking
    ///////////////////

    private UnitController FindTarget(UnitController unit, Monster monster, MonsterAI turn) {
        UnitController target = null;
        turn.targetPriority.ForEach((MonsterTarget targetType) => {
            if (target != null) {
                return;
            }
            switch (targetType) {
                case MonsterTarget.FOCUSED:
                    target = monster.focusedTarget;
                    break;

                case MonsterTarget.FACING:
                    target = FindFacingTarget(unit);
                    break;
            }
        });

        return target;
    }

    private UnitController FindFacingTarget(UnitController unit) {
        Dictionary<UnitController, float> unitToDistance = new Dictionary<UnitController, float>();
        UnitManager.instance.Units.ForEach(otherUnit => {
            if (otherUnit.myPlayer.faction != unit.myPlayer.faction) {
                //TODO add LoS check
                if (unit.facingDirection == unit.GetDirectionToTile(otherUnit.myTile)) {
                    //TODO change this to move distance
                    Vector2 my2DPosition = new Vector2(unit.myTile.X, unit.myTile.Y);
                    Vector2 target2DPosition = new Vector2(otherUnit.myTile.X, otherUnit.myTile.Y);
                    float distanceToTarget = Vector2.Distance(my2DPosition, target2DPosition);

                    unitToDistance.Add(otherUnit, distanceToTarget);
                }
            }
        });

        UnitController closestFacedUnit = unitToDistance.First().Key;
        unitToDistance.Keys.ToList().ForEach((otherUnit) => {
            if (unitToDistance[otherUnit] < unitToDistance[closestFacedUnit]) {
                closestFacedUnit = otherUnit;
            }
        });

        return closestFacedUnit;
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
                UnitManager.instance.SetUnitPath(unit, pathToTarget);
            }
            unit.ActionPoints--;
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
        }

        AttackTile(unit, target.myTile.Nodes.First(), action.Attack);
        unit.ActionPoints--;

        yield return TurnManager.instance.WaitForWaitingForInput();
    }

    public void AttackTile(UnitController unit, Node targetTile, AttackAction attack) {
        // gets the first target of the first ability
        //Debug.Log("AI - attacking tile: " + targetTile);
        UnitManager.instance.AttackTile(unit, targetTile, attack);
    }
}