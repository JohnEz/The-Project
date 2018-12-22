using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class AIManager : MonoBehaviour {

    public static AIManager singleton;

	List<UnitController> myUnits;

	TileMap myMap;

    private void Awake() {
        singleton = this;
    }

    // Use this for initialization
    void Start () {

	}

	// Update is called once per frame
	void Update () {
		
	}

	public void Initialise(TileMap map) {
		myMap = map;
	}

	public List<UnitController> MyUnits {
		get { return myUnits; }
		set { myUnits = value; }
	}

	// NewTurn is called at the start of each of the AIs turns.
	public IEnumerator NewTurn(int myPlayerId) {
		myUnits = UnitManager.singleton.Units.Where (unit => unit.myPlayer.id == myPlayerId).ToList ();

		foreach (UnitController unit in myUnits) {
            CameraManager.singleton.FollowTarget(unit.transform);
            yield return PlanTurnTwoActions(unit);
		}

		TurnManager.singleton.EndTurn ();
	}

    //If we only want AI to have a single move and a single attack
	public IEnumerator PlanTurnMoveAndAttack(UnitController unit) {
        bool hasEndedTurn = false;
        bool hasAttacked = false;
        bool hasMoved = false;

        while (!hasEndedTurn) {
            yield return WaitForWaitingForInput();

            Dictionary<AttackAction, List<Node>> potentialAbilityTargets = GetPotentialAbilityTargets(unit);
            bool hasAvailableTargets = potentialAbilityTargets.Where(keyValuePair => keyValuePair.Value.Count > 0).ToList().Count > 0;
            AttackAction firstAttack = unit.myStats.Attacks[0];

            // if the first ability has 1 or more available targets
            if (!hasAttacked && hasAvailableTargets) {
                AttackTile(unit, potentialAbilityTargets[firstAttack][0], firstAttack);
                hasAttacked = true;
            } else if (!hasMoved && !hasAvailableTargets && unit.myStats.Speed > 0) {
                // if the unit can move

                // find paths to all enemies
                List<MovementPath> paths = FindPathsToEnemies(unit);

                // if there is a valid path
                if (paths.Count > 0) {
                    MoveShortestPath(unit, paths);
                } else {
                    // no options available
                    // end unit turn
                }
                hasMoved = true;
            } else {
                hasEndedTurn = true;
            }

        }
        yield return WaitForWaitingForInput ();

	}

    public IEnumerator PlanTurnTwoActions(UnitController unit) {
        int actionPoints = 2;

        while (actionPoints > 0) {
            yield return WaitForWaitingForInput();

            Dictionary<AttackAction, List<Node>> potentialAbilityTargets = GetPotentialAbilityTargets(unit);
            Dictionary<AttackAction, List<Node>> filteredPotentialAbilityTargets = potentialAbilityTargets.Where(keyValuePair => keyValuePair.Value.Count > 0).ToDictionary(i => i.Key, i => i.Value);

            bool hasAvailableTargets = filteredPotentialAbilityTargets.Keys.Count > 0;

            // if the first ability has 1 or more available targets
            if (hasAvailableTargets) {

                // TODO THIS IS DUMB
                int chosenAttackIndex = Random.Range(0, filteredPotentialAbilityTargets.Keys.Count);
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

                    List<Node> nodesToAttackFrom = myMap.pathfinder.FindAttackableTiles(otherUnit.myNode, attackAction);

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
            MovementPath pathToNode = myMap.pathfinder.FindPath(unit.myNode, targetNode, unit.myStats.walkingType, unit.myPlayer.faction);
            if (pathToNode.movementCost != -1) {
                pathsToNodes.Add(pathToNode);
            }
        });

        return pathsToNodes;
    }

        // finds finds the shortest path to all enemies
    public List<MovementPath> FindPathsToEnemies(UnitController unit) {
        List<MovementPath> pathsToEnemies = new List<MovementPath>();

        UnitManager.singleton.Units.ForEach(otherUnit => {
            if (otherUnit.myPlayer.faction != unit.myPlayer.faction) {
                MovementPath pathToEnemy = myMap.pathfinder.FindShortestPathToUnit(unit.myNode, otherUnit.myNode, unit.myStats.walkingType, unit.myPlayer.faction);
                if (pathToEnemy.movementCost != -1) {
                    pathsToEnemies.Add(pathToEnemy);
                }
            }
        });

        return pathsToEnemies;
    }

    // Finds all units currently attackable from the units tile
    public Dictionary<AttackAction, List<Node>> GetPotentialAbilityTargets(UnitController unit) {
        Dictionary<AttackAction, List<Node>> potentialAbilityTargets = new Dictionary<AttackAction, List<Node>>();

        unit.myStats.Attacks.ForEach(attackAction => {
            if (attackAction.IsOnCooldown()) {
                return;
            }

            List<Node> attackableTiles = myMap.pathfinder.FindAttackableTiles(unit.myNode, attackAction);

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

    public void MoveShortestPath(UnitController unit, List<MovementPath> paths) {
        // finds the shortest path out of all the paths
        MovementPath shortestPath = Pathfinder.GetSortestPath(paths);
        shortestPath.path = shortestPath.path.Take(unit.myStats.Speed).ToList();

        //Debug.Log("I want to move to node: " + shortestPath.path.Last());

        // if there is a unit on the final node
        if (shortestPath.path.Last().myUnit != null) {
            //remove that node
            shortestPath.path.Remove(shortestPath.path.Last());
        }

        //Debug.Log("I am moving to node: " + shortestPath.path.Last());

        // We need to refind the path to this node or clean up the path
        // this is because the path neighbours could have been overriten
        // path should not use any none static node data in future
        shortestPath.path = Pathfinder.CleanPath(shortestPath.path, unit.myNode);

        UnitManager.singleton.SetUnitPath(unit, shortestPath);
    }

	public IEnumerator WaitForWaitingForInput() {
		return new WaitUntil (() => TurnManager.singleton.CurrentPhase == TurnPhase.WAITING_FOR_INPUT);
	}
}
