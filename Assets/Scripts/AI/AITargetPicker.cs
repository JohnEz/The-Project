using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AITargetPicker {
    private static AITargetPicker instance = null;

    // target and its calculated target value
    public Dictionary<UnitController, int> targets;

    private AITargetPicker() {
        targets = new Dictionary<UnitController, int>();
    }

    public static AITargetPicker Instance {
        get {
            if (instance == null) {
                instance = new AITargetPicker();
            }
            return instance;
        }
    }

    public Dictionary<Node, int> GetValueToMoveToNodes(UnitController unit, List<Node> nodes, bool includeHostility = false) {
        Dictionary<Node, int> nodeValues = new Dictionary<Node, int>();

        nodes.ForEach(node => {
            MovementPath shortestPathToNode = TileMap.instance.pathfinder.FindPath(unit.myNode, node, unit.myStats.walkingType, unit.myPlayer.faction);

            if (shortestPathToNode.movementCost > -1) {
                nodeValues.Add(node, ConvertPathToValue(unit, shortestPathToNode, includeHostility));
            }
        });

        return nodeValues;
    }

    public UnitController GetMyEnemyTarget(UnitController unit, bool isMeleeOnly) {
        targets.Clear();

        Dictionary<UnitController, MovementPath> pathsToEnemies = FindPathsToEnemies(unit);

        pathsToEnemies.Keys.ToList().ForEach(enemyUnit => {
            targets.Add(enemyUnit, ConvertPathToValue(unit, pathsToEnemies[enemyUnit]));
        });

        // TODO add additional logic around previous target and prefer them

        // TODO add additional logic around health of target

        // TODO add additonal logic around armour of target

        // get the unit with the highest value
        UnitController targetUnit = AIManager.GetBestOption(targets);

        return targetUnit;
    }

    private int ConvertPathToValue(UnitController unit, MovementPath path, bool includeHostility = false) {
        Node targetNode = path.movementCost == 0 ? unit.myNode : path.path.Last();
        TileHostility hostility = AIInfoCollector.Instance.GetHostilityOfTile(unit.myPlayer.faction, targetNode);
        int damageReductionFromArmour = hostility.numberOfAttacks * unit.myStats.Armour;
        int potentialDamageTaken = includeHostility ? hostility.heat - damageReductionFromArmour : 0;
        //return -path.movementCost - potentialDamageTaken;
        //return -Mathf.CeilToInt((float)path.movementCost / (float)unit.myStats.Speed);
        return -Mathf.CeilToInt((float)path.movementCost / (float)unit.myStats.Speed) - potentialDamageTaken;
    }

    // Finds all tiles that can attack an enemy
    private List<Node> FindPossibleAttackNodes(UnitController unit) {
        List<Node> attackNodes = new List<Node>();

        UnitManager.instance.Units.ForEach(otherUnit => {
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

    private List<MovementPath> FindShortestPathsToNodes(UnitController unit, List<Node> targetNodes) {
        List<MovementPath> pathsToNodes = new List<MovementPath>();

        targetNodes.ForEach(targetNode => {
            MovementPath pathToNode = TileMap.instance.pathfinder.FindPath(unit.myNode, targetNode, unit.myStats.walkingType, unit.myPlayer.faction);
            if (pathToNode.movementCost != -1) {
                pathsToNodes.Add(pathToNode);
            }
        });

        return pathsToNodes;
    }

    // finds the shortest path to all enemies
    private Dictionary<UnitController, MovementPath> FindPathsToEnemies(UnitController unit) {
        Dictionary<UnitController, MovementPath> pathsToEnemies = new Dictionary<UnitController, MovementPath>();

        UnitManager.instance.Units.ForEach(otherUnit => {
            if (otherUnit.myPlayer.faction != unit.myPlayer.faction) {
                MovementPath pathToEnemy = TileMap.instance.pathfinder.FindShortestPathToUnit(unit.myNode, otherUnit.myNode, unit.myStats.walkingType, unit.myPlayer.faction);
                // if there was a path found
                if (pathToEnemy.movementCost != -1) {
                    pathsToEnemies.Add(otherUnit, pathToEnemy);
                }
            }
        });

        return pathsToEnemies;
    }
}