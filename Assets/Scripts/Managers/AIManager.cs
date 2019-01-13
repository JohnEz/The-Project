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
    public static AIManager singleton;

    private List<UnitController> myUnits;

    private void Awake() {
        singleton = this;
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

    public static T GetBestOption<T>(Dictionary<T, int> options) {
        return options.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
    }

    public static T GetBestOption<T>(Dictionary<T, int> options, Func<KeyValuePair<T, int>, KeyValuePair<T, int>, KeyValuePair<T, int>> comparator) {
        return options.Aggregate(comparator).Key;
    }

    // NewTurn is called at the start of each of the AIs turns.
    public IEnumerator NewTurn(int myPlayerId) {
        myUnits = UnitManager.singleton.Units.Where(unit => unit.myPlayer.id == myPlayerId).ToList();
        // TODO make this async
        int faction = PlayerManager.singleton.GetPlayer(myPlayerId).faction;
        AIInfoCollector.Instance.GenerateHostilityMap(faction);

        // Debug
        //AIInfoCollector.Instance.GetHotNodes(faction).ForEach(nodePos => {
        //    TileHostility hostility = AIInfoCollector.Instance.GetHostilityOfTile(faction, nodePos);
        //    TileMap.instance.GetNode(nodePos).GetComponentInChildren<TileHighlighter>().DebugSetText(hostility.heat + ", " + hostility.numberOfAttacks);
        //});

        foreach (UnitController unit in myUnits) {
            CameraManager.singleton.FollowTarget(unit.transform);
            //yield return PlanTurnTwoActions(unit);
            yield return ExecutePlannedTurn(unit);
        }

        AIInfoCollector.Instance.GetHotNodes(faction).ForEach(nodePos => {
            TileMap.instance.GetNode(nodePos).GetComponentInChildren<TileHighlighter>().DebugSetText("");
        });

        TurnManager.singleton.EndTurn();
    }

    public IEnumerator ExecutePlannedTurn(UnitController unit) {
        AITurnPlan turnPlan = null;
        int actionPoints = 2;
        while (actionPoints > 0) {
            yield return WaitForWaitingForInput();

            //if (turnPlan.targetMoveNode == null && turnPlan.attack == null) {
            if (turnPlan == null) {
                turnPlan = AIAttackPicker.Instance.GetBestPlan(unit);
            }

            if (turnPlan == null) {
                // No actions to take
                actionPoints = 0;
            } else if (turnPlan.targetMoveNode != null) {

                // At the target
                if (turnPlan.targetMoveNode == unit.myNode) {
                    turnPlan.targetMoveNode = null;
                } else {
                    // Move towards target Node
                    MovementPath pathToNode = TileMap.instance.pathfinder.FindPath(unit.myNode, turnPlan.targetMoveNode, unit.myStats.walkingType, unit.myPlayer.faction);

                    // Check there is a path to that node
                    if (pathToNode.movementCost > -1) {
                        // Trim path to our walkable distance
                        TripPathToWalkable(unit, pathToNode);
                        pathToNode.path = Pathfinder.CleanPath(pathToNode.path, unit.myNode);

                        // Tell unit to follow path
                        //UnitManager.singleton.SetUnitPath(unit, pathToNode);
                        Debug.Log("Told unit " + unit.myStats.className + " to go to " + pathToNode.path.Last().ToString());
                        UnitManager.singleton.SetUnitPath(unit, pathToNode);
                        actionPoints = 0;
                    } else {
                        Debug.LogError(String.Format("Unit \"{0}\" cant move to node {1}", unit.name, turnPlan.targetMoveNode));
                    }
                }

            } else if (turnPlan.attack != null) {
                AttackTile(unit, turnPlan.attack.targetNode, turnPlan.attack.attack);
                turnPlan.attack = null;
                actionPoints--;
            }

            if (turnPlan != null && turnPlan.targetMoveNode == null && turnPlan.attack == null) {
                turnPlan = null;
            }
            yield return WaitForWaitingForInput();
        }
    }

    //If we only want AI to have a single move and a single attack
    //public IEnumerator PlanTurnMoveAndAttack(UnitController unit) {
    //    bool hasEndedTurn = false;
    //    bool hasAttacked = false;
    //    bool hasMoved = false;

    //    while (!hasEndedTurn) {
    //        yield return WaitForWaitingForInput();

    //        Dictionary<AttackAction, List<Node>> potentialAbilityTargets = GetPotentialAbilityTargets(unit);
    //        bool hasAvailableTargets = potentialAbilityTargets.Where(keyValuePair => keyValuePair.Value.Count > 0).ToList().Count > 0;
    //        AttackAction firstAttack = unit.myStats.Attacks[0];

    //        // if the first ability has 1 or more available targets
    //        if (!hasAttacked && hasAvailableTargets) {
    //            AttackTile(unit, potentialAbilityTargets[firstAttack][0], firstAttack);
    //            hasAttacked = true;
    //        } else if (!hasMoved && !hasAvailableTargets && unit.myStats.Speed > 0) {
    //            // if the unit can move

    //            // find paths to all enemies
    //            List<MovementPath> paths = FindPathsToEnemies(unit);

    //            // if there is a valid path
    //            if (paths.Count > 0) {
    //                MoveShortestPath(unit, paths);
    //            } else {
    //                // no options available
    //                // end unit turn
    //            }
    //            hasMoved = true;
    //        } else {
    //            hasEndedTurn = true;
    //        }
    //    }
    //    yield return WaitForWaitingForInput();
    //}

    public IEnumerator PlanTurnTwoActions(UnitController unit) {
        // TODO move to unit
        int actionPoints = 2;

        while (actionPoints > 0) {
            yield return WaitForWaitingForInput();

            Dictionary<AttackAction, List<Node>> potentialAbilityTargets = GetPotentialAbilityTargets(unit);
            Dictionary<AttackAction, List<Node>> filteredPotentialAbilityTargets = potentialAbilityTargets.Where(keyValuePair => keyValuePair.Value.Count > 0).ToDictionary(i => i.Key, i => i.Value);

            bool hasAvailableTargets = filteredPotentialAbilityTargets.Keys.Count > 0;

            // if the first ability has 1 or more available targets
            if (hasAvailableTargets) {
                // TODO THIS IS DUMB
                int chosenAttackIndex = UnityEngine.Random.Range(0, filteredPotentialAbilityTargets.Keys.Count);
                AttackAction chosenAttack = filteredPotentialAbilityTargets.Keys.ToArray()[chosenAttackIndex];

                AttackTile(unit, filteredPotentialAbilityTargets[chosenAttack][0], chosenAttack);
                actionPoints--;
            } else if (unit.myStats.Speed > 0) {
                // if the unit can move

                // find all the nodes i can attack an enemy from
                List<Node> attackNodes = FindPossibleAttackNodes(unit, null);
                // find all the paths to these attack nodes
                List<MovementPath> paths = FindShortestPathsToNodes(unit, attackNodes);

                // if there is a valid path
                if (paths.Count > 0) {
                    MoveShortestPath(unit, paths);
                } else {
                    // no options available
                    // end unit turn
                }
                actionPoints--;
            } else {
                actionPoints = 0;
            }
        }
        yield return WaitForWaitingForInput();
    }

    // Finds all tiles that can attack an enemy
    public List<Node> FindPossibleAttackNodes(UnitController unit, AttackAction attackAction2) {
        List<Node> attackNodes = new List<Node>();

        UnitManager.singleton.Units.ForEach(otherUnit => {
            unit.myStats.Attacks.ForEach(attackAction => {
                if (attackAction.CanHitUnit(otherUnit.myNode)) {
                    List<Node> nodesToAttackFrom = TileMap.instance.pathfinder.FindAttackableTiles(otherUnit.myNode, attackAction);

                    // This isnt needed anymore but better safe than sorry i guess
                    List<Node> filteredNodesToAttackFrom = nodesToAttackFrom.Where(node => Pathfinder.UnitCanStandOnTile(node, unit.myStats.WalkingType)).ToList();

                    filteredNodesToAttackFrom.ForEach(attackNode => {
                        if (attackNode.myUnit == null || attackNode.myUnit == unit) {
                            attackNodes.Add(attackNode);
                        }
                    });
                }
            });
        });

        return attackNodes;
    }

    public List<MovementPath> FindShortestPathsToNodes(UnitController unit, List<Node> targetNodes) {
        List<MovementPath> pathsToNodes = new List<MovementPath>();

        targetNodes.ForEach(targetNode => {
            MovementPath pathToNode = TileMap.instance.pathfinder.FindPath(unit.myNode, targetNode, unit.myStats.walkingType, unit.myPlayer.faction);
            if (pathToNode.movementCost != -1) {
                pathsToNodes.Add(pathToNode);
            }
        });

        return pathsToNodes;
    }

    // Finds all units currently attackable from the units tile
    public Dictionary<AttackAction, List<Node>> GetPotentialAbilityTargets(UnitController unit) {
        Dictionary<AttackAction, List<Node>> potentialAbilityTargets = new Dictionary<AttackAction, List<Node>>();

        unit.myStats.Attacks.ForEach(attackAction => {
            if (attackAction.IsOnCooldown()) {
                return;
            }

            List<Node> attackableTiles = TileMap.instance.pathfinder.FindAttackableTiles(unit.myNode, attackAction);

            attackableTiles = attackableTiles.Where(tile => attackAction.CanHitUnit(tile)).ToList();

            potentialAbilityTargets.Add(attackAction, attackableTiles);
        });

        return potentialAbilityTargets;
    }

    public void AttackTile(UnitController unit, Node targetTile, AttackAction attack) {
        // gets the first target of the first ability
        //Debug.Log("AI - attacking tile: " + targetTile);
        UnitManager.singleton.AttackTile(unit, targetTile, attack);
    }

    private void TripPathToWalkable(UnitController unit, MovementPath movementPath) {
        movementPath.path = movementPath.path.Take(unit.myStats.Speed).ToList();

        //Debug.Log("I want to move to node: " + shortestPath.path.Last());

        // if there is a unit on the final node
        while (movementPath.path.Last().myUnit != null) {
            //remove that node
            movementPath.path.Remove(movementPath.path.Last());
        }
    }

    public void MoveShortestPath(UnitController unit, List<MovementPath> paths) {
        // finds the shortest path out of all the paths
        MovementPath shortestPath = Pathfinder.GetSortestPath(paths);
        TripPathToWalkable(unit, shortestPath);

        //Debug.Log("I am moving to node: " + shortestPath.path.Last());

        // We need to refind the path to this node or clean up the path
        // this is because the path neighbours could have been overriten
        // path should not use any none static node data in future
        shortestPath.path = Pathfinder.CleanPath(shortestPath.path, unit.myNode);

        UnitManager.singleton.SetUnitPath(unit, shortestPath);
    }

    public IEnumerator WaitForWaitingForInput() {
        return new WaitUntil(() => TurnManager.singleton.CurrentPhase == TurnPhase.WAITING_FOR_INPUT);
    }
}