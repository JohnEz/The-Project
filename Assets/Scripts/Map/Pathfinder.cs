using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public struct MovementPath {
	public List<Node> path;
	//public List<Node> dashPath;
	public int movementCost;
}

public struct ReachableTiles {
	public Dictionary<Node, float> basic;
	public Dictionary<Node, float> extended;
}

public struct MovementAndAttackPath {
	public ReachableTiles movementTiles;
	public List<Node> attackTiles;
}

public class Pathfinder : MonoBehaviour {

	TileMap map;

	// Use this for initialization
	public void Initialise () {
		map = GetComponent<TileMap> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public static List<Node> CleanPath(List<Node> path, Node startingNode) {
        List<Node> cleanedPath = new List<Node>();

        Node previousNode = startingNode;

        path.ForEach(node => {
            Neighbour previousNeighbour = new Neighbour();
            previousNeighbour.node = previousNode;
            previousNeighbour.direction = DirectionToNode(node, previousNode);
            node.previous = previousNeighbour;

            previousNode = node;
            cleanedPath.Add(node);

        });

        return cleanedPath;
    }

    public static Vector2 DirectionToNode(Node from, Node to) {
        return new Vector2(Math.Sign(from.x - to.x), Math.Sign(to.y - from.y));
    }

	public ReachableTiles findReachableTiles(Node startNode, float distance, Walkable walkingType, int faction, bool canDash = false) {
		map.resetTiles ();

		List<Node> openList = new List<Node> ();
		List<Node> currentList;
		Dictionary<Node, float> reachableNodes = new Dictionary<Node, float> ();

		float maxDistance = canDash ? distance*2 : distance;

		bool changed = true;

		startNode.cost = 0;
		openList.Add (startNode);

		while (changed) {
			changed = false;
			currentList = openList;
			openList = new List<Node> ();

			foreach (Node node in currentList) {

				foreach (Neighbour neighbour in node.neighbours) {
					
					if (neighbour.node != startNode && IsTileWalkable(node, neighbour.node, walkingType, faction) && !neighbour.hasDoor) {
						float totalCost = node.cost + neighbour.node.moveCost;

						if (totalCost <= maxDistance) {
							if (!reachableNodes.ContainsKey (neighbour.node) || reachableNodes[neighbour.node] > totalCost) {
								neighbour.node.previous = new Neighbour();
								neighbour.node.previous.node = node;
								neighbour.node.previous.direction = neighbour.direction;
								neighbour.node.cost = totalCost;
								reachableNodes [neighbour.node] = totalCost;
								openList.Add (neighbour.node);
								changed = true;
							}
						}

					}

				}
			}

		}
			
		//if you cant end your path on a unit THIS IS DANGEROUS
		if (faction != -1) {
			reachableNodes = reachableNodes.Where (item => item.Key.myUnit == null).ToDictionary (item => item.Key, item => item.Value);
		}

		ReachableTiles reachableTiles;
		reachableTiles.basic = reachableNodes.Where (item => item.Key.cost <= distance).ToDictionary (item => item.Key, item => item.Value);
		reachableTiles.extended = reachableNodes.Where (item => item.Key.cost > distance).ToDictionary (item => item.Key, item => item.Value);

		return reachableTiles;
	}

	public MovementPath FindShortestPathToUnit(Node source, Node target, Walkable walkingType, int faction) {
		List<MovementPath> paths = new List<MovementPath> ();
		map.resetTiles ();
		target.neighbours.ForEach (neighbour => {
			if (neighbour.node.myUnit == null && UnitCanStandOnTile(neighbour.node, walkingType)) {
				paths.Add(FindPath(source, neighbour.node, walkingType, faction));
			}
		});

		/*List<MovementPath> pathsWithoutUnitsAtEnd = paths.Where (movementPath => {
			return movementPath.movementCost > -1 && 
				movementPath.path [movementPath.path.Count-1].myUnit == null;
		}).ToList();*/


		//return GetSortestPath (pathsWithoutUnitsAtEnd.Count > 0 ? pathsWithoutUnitsAtEnd : paths);
		return GetSortestPath (paths);
	}

	public static MovementPath GetSortestPath(List<MovementPath> paths) {
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

    // this finds the path ONTO a tile
	public MovementPath FindPath(Node source, Node target, Walkable walkingType, int faction) {
		map.resetTiles ();

		List<Node> openList = new List<Node> ();
		MovementPath path = new MovementPath ();
		path.movementCost = -1;

		source.cost = 0;

		openList.Add (source);

		while (openList.Count > 0) {

			//find the current lowest cost tile
			Node currentNode = null;
			foreach (Node n in openList) {
				if (currentNode == null || n.Value < currentNode.Value) {
					currentNode = n;
				}
			}

			if (currentNode == null || currentNode == target) {
				break; // i hate just calling break, lets change to bool?
			}

			openList.Remove (currentNode);

			foreach (Neighbour neighbour in currentNode.neighbours) {
                if (neighbour.node != source && IsTileWalkable(currentNode, neighbour.node, walkingType, faction) && !neighbour.hasDoor) {
					float totalCost = currentNode.cost + neighbour.node.moveCost;

					if (neighbour.node.cost > totalCost) {
						neighbour.node.previous = new Neighbour();
						neighbour.node.previous.node = currentNode;
						neighbour.node.previous.direction = neighbour.direction;
						neighbour.node.cost = totalCost;
						openList.Add (neighbour.node);
					}
				}
			}
		}

		if (target.previous.node != null) {
			path = getPathFromTile (target);
		}

		return path;
	}

	public MovementPath getPathFromTile (Node endNode) {
		Node currentNode = endNode;
		MovementPath newPath = new MovementPath ();
		newPath.path = new List<Node> ();
		newPath.movementCost = (int)endNode.cost;

		newPath.path.Add (endNode);

		//while there is a previous node
		while (currentNode.previous.node != null) {
			Node previousNode = currentNode.previous.node;
			newPath.path.Add (previousNode);
			currentNode = previousNode;
		}

		//need to remove the last added node as that was the starting node
		newPath.path.RemoveAt(newPath.path.Count-1);

		newPath.path.Reverse ();

		return newPath;
	}

	//public MovementAndAttackPath findMovementAndAttackTiles(UnitController unit, AttackAction action, int actions) {
	//	bool canDash = actions > 1;
	//	MovementAndAttackPath reachableTiles;
	//	reachableTiles.movementTiles = findReachableTiles (unit.myNode, unit.myStats.Speed, unit.myStats.WalkingType, unit.myPlayer.faction, canDash);
	//	reachableTiles.attackTiles = new List<Node> ();

	//	//Need to do a test for the starting tile
	//	foreach (Neighbour neighbour in unit.myNode.neighbours) {
	//		UnitController targetUnit = neighbour.node.myUnit;
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
	//						UnitController targetUnit = neighbour.node.myUnit;
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

	public static bool IsTileWalkable(Node startNode, Node endNode, Walkable walkingType, int faction) {
		return UnitCanStandOnTile(endNode, walkingType) && !UnitInTheWay(endNode, faction) && UnitCanChangeLevel(startNode, endNode, walkingType);
	}

	public static bool UnitCanStandOnTile(Node node, Walkable walkingType) {
		return node.walkable <= walkingType;
	}

	public static bool UnitInTheWay(Node node, int faction) {
		return node.myUnit != null && node.myUnit.myPlayer.faction != faction && faction != -1;
	}

	public static bool UnitCanChangeLevel(Node startNode, Node endNode, Walkable walkingType) {
		int levelDifference = Math.Abs(startNode.height - endNode.height);
		int maxDifference = (int)walkingType;

		return levelDifference <= maxDifference + 1;
	}

	public List<Node> FindAttackableTiles(Node node, AttackAction action) {
		switch (action.areaOfEffect) {
		case AreaOfEffect.AURA:
			return FindAOEHitTiles (node, action);
		case AreaOfEffect.CIRCLE:
		case AreaOfEffect.CLEAVE:
			return FindCircleTargetTiles (node, action);
		case AreaOfEffect.SINGLE:
		default:
			return FindSingleTargetTiles(node, action);
		}

	}

	List<Node> FindSingleTargetTiles(Node startNode, AttackAction action) {
		List<Node> reachableTiles = findReachableTiles (startNode, action.range, Walkable.Flying, -1).basic.Keys.ToList();
		if (action.CanTargetSelf) {
			reachableTiles.Insert (0, startNode);
		}

        reachableTiles = reachableTiles.FindAll(node => HasLineOfSight(startNode, node));

		return reachableTiles;
	}

	List<Node> FindCircleTargetTiles(Node startNode, AttackAction action) {
        List<Node> reachableTiles = FindSingleTargetTiles(startNode, action);

        return reachableTiles.Where(tile => action.CanTargetTile(tile)).ToList();
	}

	public List<Node> FindAOEHitTiles(Node node, AttackAction action) {
		List<Node> targetTiles = findReachableTiles (node, action.aoeRange, Walkable.Flying, -1).basic.Keys.ToList();
		targetTiles.Insert (0, node);
		return targetTiles;
	}

	public List<Node> FindCleaveTargetTiles(Node node, AttackAction action, Node start) {
		//TODO write a smarter way of doing this
		bool attackingHorizontally = start.x != node.x;
		List<Node> targetTiles = findReachableTiles (node, action.aoeRange, Walkable.Flying, -1).basic.Keys.ToList();
		List<Node> removeTiles = new List<Node>();
		targetTiles.Insert (0, node);
		if (attackingHorizontally) {
			targetTiles.ForEach ((tile) => {
				if (tile.x != node.x) {
					removeTiles.Add(tile);
				}
			});
		} else {
			targetTiles.ForEach ((tile) => {
				if (tile.y != node.y) {
					removeTiles.Add(tile);
				}
			});
		}

		removeTiles.ForEach ((tile) => {
			targetTiles.Remove(tile);
		});

		return targetTiles;
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
            Node nextNode = map.getNode(x, y);
            Neighbour neighbourBetweenNodes = previousNode.neighbours.Find(neighbour => neighbour.node == nextNode);

            //Debug.Log(neighbourBetweenNodes.ToString());

            if (neighbourBetweenNodes == null) {
                //Debug.Log("No neighbour found between nodes");
            }

            if (map.getNode(x, y).lineOfSight != LineOfSight.Full || (neighbourBetweenNodes != null && neighbourBetweenNodes.hasDoor)) {
                return false;
            }

            previousNode = nextNode;
        }

        return true;
    }

		
}
