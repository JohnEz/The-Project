using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct MovementPath {
    public List<Tile> path;

    //public List<Node> dashPath;
    public int movementCost;
}

public struct ReachableTiles {
    public Tile startingTile;
    public Dictionary<Tile, float> basic;
    public Dictionary<Tile, float> extended;

    public static List<Node> GetBasicNodes(ReachableTiles tiles) {
        return GetNodesFromTileList(tiles.basic.Keys.ToList(), tiles.startingTile);
    }

    public static List<Node> GetExtendedNodes(ReachableTiles tiles) {
        return GetNodesFromTileList(tiles.extended.Keys.ToList(), tiles.startingTile);
    }

    private static List<Node> GetNodesFromTileList(List<Tile> tiles, Tile startingTile) {
        List<Node> reachableNodes = new List<Node>();

        tiles.ToList().ForEach((Tile tile) => {
            tile.Nodes.ForEach((Node node) => {
                if (!reachableNodes.Contains(node) && !startingTile.Nodes.Contains(node)) {
                    reachableNodes.Add(node);
                }
            });
        });

        return reachableNodes;
    }
}

public struct MovementAndAttackPath {
    public ReachableTiles movementTiles;
    public List<Node> attackTiles;
}

public class PathSearchOptions {
    public bool canDash = false;
    public bool canPassThroughAllies = true;
    public int faction = -1;
    public UnitSize size = UnitSize.SMALL;

    public PathSearchOptions() {
    }

    public PathSearchOptions(int _faction, UnitSize _size) {
        faction = _faction;
        size = _size;
    }

    public PathSearchOptions(bool _canDash, bool _canPassThroughAllies, int _faction, UnitSize _size) {
        canDash = _canDash;
        canPassThroughAllies = _canPassThroughAllies;
        faction = _faction;
        size = _size;
    }
}

public class Pathfinder : MonoBehaviour {
    private TileMap map;

    // Use this for initialization
    public void Initialise() {
        map = GetComponent<TileMap>();
    }

    // Update is called once per frame
    private void Update() {
    }

    // cleans up the "previous" variables of a list of nodes to form a path
    public static List<Tile> CleanPath(List<Tile> path, Tile startingTile) {
        if (path.Count == 0) {
            return path;
        }

        List<Tile> cleanedPath = new List<Tile>();

        Tile previousTile = startingTile;

        path.ForEach(tile => {
            tile.previous = tile.FindNeighbourTo(previousTile);

            previousTile = tile;
            cleanedPath.Add(tile);
        });

        return cleanedPath;
    }

    public static Vector2 DirectionToNode(Node from, Node to) {
        return new Vector2(Math.Sign(from.x - to.x), Math.Sign(to.y - from.y));
    }

    public MovementPath getPathFromTile(Tile endTile) {
        Tile currentTile = endTile;
        MovementPath newPath = new MovementPath();
        newPath.path = new List<Tile>();
        newPath.movementCost = (int)endTile.cost;

        newPath.path.Add(endTile);

        //while there is a previous node
        while (currentTile.previous != null) {
            Tile previousTile = currentTile.previous.GetOppositeTile(currentTile);
            newPath.path.Add(previousTile);
            currentTile = previousTile;
        }

        //need to remove the last added node as that was the starting node
        newPath.path.RemoveAt(newPath.path.Count - 1);

        newPath.path.Reverse();

        return newPath;
    }

    public static MovementPath GetShortestDestination(List<MovementPath> paths) {
        MovementPath shortestPath = new MovementPath();
        shortestPath.movementCost = -1;
        bool foundPath = false;

        foreach (MovementPath path in paths) {
            if (path.movementCost != -1 && (!foundPath || path.movementCost < shortestPath.movementCost)) {
                shortestPath = path;
                foundPath = true;
            }
        }

        //shortestPath.path.RemoveAt (shortestPath.path.Count - 1);

        return shortestPath;
    }

    // find all nodes that can be reached
    public ReachableTiles findReachableTiles(Tile startTile, float distance, WalkableLevel walkingType, PathSearchOptions pSOptions) {
        PathSearchOptions options = pSOptions == null ? new PathSearchOptions() : pSOptions;
        map.resetTiles();

        List<Tile> openList = new List<Tile>();
        List<Tile> currentList;
        Dictionary<Tile, float> reachableNodes = new Dictionary<Tile, float>();

        float maxDistance = options.canDash ? distance * 2 : distance;

        bool changed = true;

        startTile.cost = 0;
        openList.Add(startTile);

        while (changed) {
            changed = false;
            currentList = openList;
            openList = new List<Tile>();

            // for each node in our current list
            foreach (Tile tile in currentList) {
                // loop through all its neighbours
                foreach (Neighbour neighbour in tile.neighbours) {
                    // get the new node from the neighbour
                    Tile neighbourTile = neighbour.GetOppositeTile(tile);

                    // if the new node is not the starting node, there isnt a door in the way and can be walked on
                    if (neighbourTile != startTile && !neighbour.HasDoor() && IsTileWalkable(tile, neighbourTile, walkingType, options)) {
                        // caclulate the cost to move here
                        float totalCost = tile.cost + neighbourTile.MoveCost;

                        // if the cost is less or equal to our distance
                        if (totalCost <= maxDistance) {
                            // check to see if we haven't already added it, and if we have, see if this way is faster to it
                            if (!reachableNodes.ContainsKey(neighbourTile) || reachableNodes[neighbourTile] > totalCost) {
                                // set the previous neighbour to the one we just used and set costs
                                neighbourTile.previous = neighbour;
                                neighbourTile.cost = totalCost;
                                reachableNodes[neighbourTile] = totalCost;
                                openList.Add(neighbourTile);
                                changed = true;
                            }
                        }
                    }
                }
            }
        }

        //if you cant end your path on a unit THIS IS DANGEROUS
        if (options.faction != -1) {
            reachableNodes = reachableNodes.Where(item => !item.Key.ContainsAUnitExcluding(startTile.MyUnit)).ToDictionary(item => item.Key, item => item.Value);
        }

        ReachableTiles reachableTiles;
        reachableTiles.startingTile = startTile;
        reachableTiles.basic = reachableNodes.Where(item => item.Key.cost <= distance).ToDictionary(item => item.Key, item => item.Value);
        reachableTiles.extended = reachableNodes.Where(item => item.Key.cost > distance).ToDictionary(item => item.Key, item => item.Value);

        return reachableTiles;
    }

    //WARNING THIS DOES NOT RETURN WORKING PATHS, ONLY DISTANCE
    public MovementPath FindShortestPathToUnit(Tile start, Tile target, WalkableLevel walkingType, PathSearchOptions pSOptions) {
        PathSearchOptions options = pSOptions == null ? new PathSearchOptions() : pSOptions;
        List<MovementPath> paths = new List<MovementPath>();
        map.resetTiles();

        // loop through all the target nodes neighbours
        target.neighbours.ForEach(neighbour => {
            // check to see if we can stand on the tile
            Tile neighbourTile = neighbour.GetOppositeTile(target);
            if ((neighbourTile.MyUnit == null || neighbourTile.MyUnit == start.MyUnit) && UnitCanStandOnTile(neighbourTile, walkingType)) {
                paths.Add(FindPath(start, neighbourTile, walkingType, options));
            }
        });

        List<MovementPath> pathsWithoutUnitsAtEnd = paths.Where(movementPath => {
            // TODO will probably need to check if tile contains unit instead
            return movementPath.movementCost == 0 || (movementPath.movementCost > 0 &&
                movementPath.path[movementPath.path.Count - 1].ContainsAUnitExcluding(start.MyUnit));
        }).ToList();

        return GetShortestDestination(pathsWithoutUnitsAtEnd.Count > 0 ? pathsWithoutUnitsAtEnd : paths);
    }

    // this finds the fastest path ONTO a tile
    public MovementPath FindPath(Tile start, Tile target, WalkableLevel walkingType, PathSearchOptions pSOptions) {
        PathSearchOptions options = pSOptions == null ? new PathSearchOptions() : pSOptions;
        MovementPath path = new MovementPath();
        path.movementCost = -1;
        Tile endTile = null;

        if (start == target || start.OverlapsTile(target)) {
            path.path = new List<Tile>();
            path.movementCost = 0;
            return path;
        }

        map.resetTiles();

        List<Tile> openList = new List<Tile>();
        start.cost = 0;

        openList.Add(start);

        // while we still have nodes to check
        while (openList.Count > 0) {
            //find the current lowest cost tile
            Tile currentTile = null;
            foreach (Tile t in openList) {
                if (currentTile == null || t.Value < currentTile.Value) {
                    currentTile = t;
                }
            }

            // if we couldnt find a node or its the target node
            if (currentTile == null || currentTile.OverlapsTile(target)) {
                endTile = currentTile;
                break;
            }

            openList.Remove(currentTile);

            // loop through all the nodes neighbours
            foreach (Neighbour neighbour in currentTile.neighbours) {
                // get the new node from the neighbour
                Tile neighbourTile = neighbour.GetOppositeTile(currentTile);

                // if the new node is not the starting node, there isnt a door in the way and can be walked on
                if (neighbourTile != start && !neighbour.HasDoor() && IsTileWalkable(currentTile, neighbourTile, walkingType, options)) {
                    // caclulate the cost to move here
                    float totalCost = currentTile.cost + neighbourTile.MoveCost;

                    // check to see if our new path is faster to this node
                    if (neighbourTile.cost > totalCost) {
                        // set the previous neighbour to the one we just used and set costs
                        neighbourTile.previous = neighbour;
                        neighbourTile.cost = totalCost;
                        openList.Add(neighbourTile);
                    }
                }
            }
        }

        // if the target tile has a previous node, we found a path
        if (endTile != null && endTile.previous != null) {
            path = getPathFromTile(endTile);
        }

        return path;
    }

    //public MovementAndAttackPath findMovementAndAttackTiles(UnitController unit, AttackAction action, int actions) {
    //	bool canDash = actions > 1;
    //	MovementAndAttackPath reachableTiles;
    //	reachableTiles.movementTiles = findReachableTiles (unit.myNode, unit.myStats.Speed, unit.myStats.WalkingType, unit.myPlayer.faction, canDash);
    //	reachableTiles.attackTiles = new List<Node> ();

    //	//Need to do a test for the starting tile
    //	foreach (Neighbour neighbour in unit.myNode.neighbours) {
    //		UnitController targetUnit = neighbour.node.MyUnit;
    //		if (targetUnit && action.CanTargetTile (neighbour.node)) {
    //			reachableTiles.attackTiles.Add (neighbour.node);
    //			neighbour.node.previous = new Neighbour ();
    //			neighbour.node.previous.node = unit.myNode;
    //			neighbour.node.previous.direction = neighbour.direction;
    //			neighbour.node.previous.node.cost = 0;
    //		}
    //	}

    //	if (actions > 1) {
    //		foreach (Node tile in reachableTiles.movementTiles.basic.Keys) {
    //			foreach (Neighbour neighbour in tile.neighbours) {
    //				//if its a tile we cant already walk on to
    //				if (!reachableTiles.movementTiles.basic.Keys.Contains (neighbour.node)) {
    //					//if its not already highligheted or if the new parent is faster than previous
    //					if (!reachableTiles.attackTiles.Contains (neighbour.node) || tile.cost < neighbour.node.previous.node.cost) {
    //						UnitController targetUnit = neighbour.node.MyUnit;
    //						if (targetUnit && action.CanTargetTile (neighbour.node)) {
    //							neighbour.node.previousMoveNode = tile;
    //							neighbour.node.previous = new Neighbour ();
    //							neighbour.node.previous.node = tile;
    //							neighbour.node.previous.direction = neighbour.direction;
    //							reachableTiles.attackTiles.Add (neighbour.node);
    //						}
    //					}
    //				}
    //			}

    //		}
    //	}
    //	return reachableTiles;
    //}

    public static bool IsTileWalkable(Tile startTile, Tile endTile, WalkableLevel walkingType, PathSearchOptions options) {
        return UnitCanStandOnTile(endTile, walkingType) && !UnitInTheWay(endTile, walkingType, options) && UnitCanChangeLevel(startTile, endTile, walkingType);
    }

    public static bool UnitCanStandOnTile(Tile tile, WalkableLevel walkingType) {
        return tile.Walkable <= walkingType;
    }

    public static bool UnitInTheWay(Tile tile, WalkableLevel walkingType, PathSearchOptions options) {
        bool unitInTheWay = false;
        tile.Nodes.ForEach((Node node) => {
            if (UnitInTheWay(node, walkingType, options, "temp")) {
                unitInTheWay = true;
            }
        });
        return unitInTheWay;
    }

    private static bool UnitInTheWay(Node node, WalkableLevel walkingType, PathSearchOptions options, string randomShit) {
        bool hasUnit = node.MyUnit != null;
        bool ignoreFaction = options.faction == -1;
        bool unitIsSameFaction = hasUnit && node.MyUnit.myPlayer.faction == options.faction;
        bool canWalkThroughUnit = ignoreFaction || (unitIsSameFaction && options.canPassThroughAllies);
        bool movementIsFlying = walkingType == WalkableLevel.Flying;

        return hasUnit && !canWalkThroughUnit && !movementIsFlying;
    }

    public static bool UnitCanChangeLevel(Tile startNode, Tile endNode, WalkableLevel walkingType) {
        //int levelDifference = Math.Abs(startNode.height - endNode.height);
        //int maxDifference = (int)walkingType;

        //return levelDifference <= maxDifference + 1;
        return true;
    }

    public List<Node> FindEffectedTiles(Tile unitTile, Node targetNode, AttackAction action) {
        switch (action.areaOfEffect) {
            //case AreaOfEffect.AURA:
            //    return FindAttackableTiles(unitNode, action);

            //case AreaOfEffect.CIRCLE:
            //    return FindAttackableTiles(targetNode, action);

            //case AreaOfEffect.CLEAVE:
            //    return FindCleaveTargetTiles(targetNode, action, unitNode);

            case AreaOfEffect.SINGLE:
            default:
                List<Node> targetTiles = new List<Node>();
                targetTiles.Add(targetNode);
                return targetTiles;
        }
    }

    public List<Node> FindAttackableTiles(Tile tile, AttackAction action) {
        switch (action.areaOfEffect) {
            //case AreaOfEffect.AURA:
            //    return FindAOEHitTiles(nodes, action);

            //case AreaOfEffect.CIRCLE:
            //case AreaOfEffect.CLEAVE:
            //    return FindCircleTargetTiles(nodes, action);

            case AreaOfEffect.SINGLE:
            default:
                return FindSingleTargetTiles(tile, action);
        }
    }

    private List<Node> FindSingleTargetTiles(Tile startTile, AttackAction action) {
        List<Tile> reachableTiles = findReachableTiles(startTile, action.range, WalkableLevel.Flying, null).basic.Keys.ToList();
        List<Node> reachableNodes = new List<Node>();

        reachableTiles.ForEach((Tile tile) => {
            tile.Nodes.ForEach((Node node) => {
                if (!reachableNodes.Contains(node)) {
                    reachableNodes.Add(node);
                }
            });
        });

        if (action.CanTargetSelf) {
            startTile.Nodes.ForEach((Node node) => {
                if (!reachableNodes.Contains(node)) {
                    reachableNodes.Add(node);
                }
            });
        }

        reachableNodes = reachableNodes.FindAll(node =>
            HasLineOfSight(startTile, node) &&
            // TODO this wont work when we have slowing terrain
            node.cost >= action.minRange
        );

        return reachableNodes;
    }

    //private List<Node> FindCircleTargetTiles(Node startNode, AttackAction action) {
    //    List<Node> reachableTiles = FindSingleTargetTiles(startNode, action);

    //    return reachableTiles.Where(tile => action.CanTargetTile(tile)).ToList();
    //}

    //public List<Node> FindAOEHitTiles(Node node, AttackAction action) {
    //    List<Node> targetTiles = findReachableTiles(node, action.aoeRange, Walkable.Flying, null).basic.Keys.ToList();
    //    targetTiles.Insert(0, node);
    //    return targetTiles;
    //}

    //public List<Node> FindCleaveTargetTiles(Node node, AttackAction action, Node start) {
    //    //TODO write a smarter way of doing this
    //    bool attackingHorizontally = start.x != node.x;
    //    List<Node> targetTiles = findReachableTiles(node, action.aoeRange, Walkable.Flying, null).basic.Keys.ToList();
    //    List<Node> removeTiles = new List<Node>();
    //    targetTiles.Insert(0, node);
    //    if (attackingHorizontally) {
    //        targetTiles.ForEach((tile) => {
    //            if (tile.x != node.x) {
    //                removeTiles.Add(tile);
    //            }
    //        });
    //    } else {
    //        targetTiles.ForEach((tile) => {
    //            if (tile.y != node.y) {
    //                removeTiles.Add(tile);
    //            }
    //        });
    //    }

    //    removeTiles.ForEach((tile) => {
    //        targetTiles.Remove(tile);
    //    });

    //    return targetTiles;
    //}

    public bool HasLineOfSight(Tile tile, Node end) {
        return tile.Nodes.Exists((Node node) => HasLineOfSight(node, end));
    }

    public bool HasLineOfSight(Node start, Node end) {
        if (start == end) {
            return true;
        }

        int deltaX = Mathf.Abs(end.x - start.x);
        int deltaY = Mathf.Abs(end.y - start.y);
        int x = start.x;
        int y = start.y;
        Node previousNode = start;

        // find out which way the x and y should be stepping for the line (ie. up or down)
        int stepX = start.x < end.x ? 1 : -1;
        int stepY = start.y < end.y ? 1 : -1;

        bool isSteep = deltaY > deltaX;

        int delta = isSteep ? 2 * deltaX - deltaY : 2 * deltaY - deltaX;

        int count = 0;

        //Debug.Log("Start:" + start.ToString());
        //Debug.Log("End:" + end.ToString());

        //Debug.Log("deltaX:" + deltaX);
        //Debug.Log("deltaY:" + deltaY);

        //while we are not at the end node and (safety) we havent gone past it
        while ((x != end.x || y != end.y) && count < deltaX + deltaY) {
            count++;

            //Debug.Log("D: " + D);

            if (delta > 0) {
                if (isSteep) {
                    x += stepX;
                    delta -= 2 * deltaY;
                } else {
                    y += stepY;
                    delta -= 2 * deltaX;
                }
            }

            if (isSteep) {
                y += stepY;
                delta += 2 * deltaX;
            } else {
                x += stepX;
                delta += 2 * deltaY;
            }
            //Debug.Log(map.getNode(x, y).ToString());

            // TODO potentially move this to its own function
            Node nextNode = map.GetNode(x, y);
            Neighbour neighbourBetweenNodes = previousNode.FindNeighbourTo(nextNode);

            //Debug.Log(neighbourBetweenNodes.ToString());

            // TODO this will always happen on diagonals
            if (neighbourBetweenNodes == null) {
                //Debug.Log("No neighbour found between nodes");
            }

            if (map.GetNode(x, y).lineOfSight != LineOfSight.Full || (neighbourBetweenNodes != null && neighbourBetweenNodes.HasDoor())) {
                return false;
            }

            previousNode = nextNode;
        }

        return true;
    }
}