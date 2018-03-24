using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public struct MovementPath {
	public List<Node> path;
	public List<Node> dashPath;
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
					
					if (neighbour.node != startNode && isTileWalkable(node, neighbour.node, walkingType, faction)) {
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

	public MovementPath FindPath(Node source, Node target, Walkable walkingType, int faction) {
		map.resetTiles ();

		List<Node> openList = new List<Node> ();
		MovementPath path = new MovementPath ();

		source.cost = 0;

		openList.Add (source);

		while (openList.Count > 0) {

			//find the current lowest cost tile
			Node currentNode = null;
			foreach (Node n in openList) {
				// TODO this may need is walkable check, but i think it can be done in adding neighbours
				if (currentNode == null || n.Value < currentNode.Value) {
					currentNode = n;
				}
			}

			if (currentNode == null || currentNode == target) {
				break; // i hate just calling break, lets change to bool?
			}

			openList.Remove (currentNode);

			foreach (Neighbour neighbour in currentNode.neighbours) {
				if ((neighbour.node != source && (isTileWalkable(currentNode, neighbour.node, walkingType, faction)) || (neighbour.node == target && UnitCanChangeLevel(currentNode, neighbour.node, walkingType)))) {
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

	public MovementAndAttackPath findMovementAndAttackTiles(UnitController unit, BaseAbility ability, int actions) {
		bool canDash = actions > 1;
		MovementAndAttackPath reachableTiles;
		reachableTiles.movementTiles = findReachableTiles (unit.myNode, unit.myStats.Speed, unit.myStats.WalkingType, unit.myPlayer.faction, canDash);
		reachableTiles.attackTiles = new List<Node> ();

		//Need to do a test for the starting tile
		foreach (Neighbour neighbour in unit.myNode.neighbours) {
			UnitController targetUnit = neighbour.node.myUnit;
			if (targetUnit && ability.CanTargetTile (neighbour.node)) {
				reachableTiles.attackTiles.Add (neighbour.node);
				neighbour.node.previous = new Neighbour ();
				neighbour.node.previous.node = unit.myNode;
				neighbour.node.previous.direction = neighbour.direction;
				neighbour.node.previous.node.cost = 0;
			}
		}

		if (actions > 1) {
			foreach (Node tile in reachableTiles.movementTiles.basic.Keys) {
				foreach (Neighbour neighbour in tile.neighbours) {

					//if its a tile we cant already walk on to
					if (!reachableTiles.movementTiles.basic.Keys.Contains (neighbour.node)) {
						//if its not already highligheted or if the new parent is faster than previous
						if (!reachableTiles.attackTiles.Contains (neighbour.node) || tile.cost < neighbour.node.previous.node.cost) {
							UnitController targetUnit = neighbour.node.myUnit;
							if (targetUnit && ability.CanTargetTile (neighbour.node)) {
								neighbour.node.previousMoveNode = tile;
								neighbour.node.previous = new Neighbour ();
								neighbour.node.previous.node = tile;
								neighbour.node.previous.direction = neighbour.direction;
								reachableTiles.attackTiles.Add (neighbour.node);
							}
						}
					}
				}
			}
		}
		return reachableTiles;
	}

	public bool isTileWalkable(Node startNode, Node endNode, Walkable walkingType, int faction) {
		return UnitCanStandOnTile(endNode, walkingType) && !UnitInTheWay(endNode, faction) && UnitCanChangeLevel(startNode, endNode, walkingType);
	}

	public bool UnitCanStandOnTile(Node node, Walkable walkingType) {
		return node.walkable <= walkingType;
	}

	public bool UnitInTheWay(Node node, int faction) {
		return node.myUnit != null && node.myUnit.myPlayer.faction != faction && faction != -1;
	}

	public bool UnitCanChangeLevel(Node startNode, Node endNode, Walkable walkingType) {
		int levelDifference = Math.Abs(startNode.level - endNode.level);
		int maxDifference = (int)walkingType;

		return levelDifference <= maxDifference + 1;
	}

	public List<Node> FindAttackableTiles(Node node, BaseAbility ability) {
		switch (ability.areaOfEffect) {
		case AreaOfEffect.AURA:
			return FindAOEHitTiles (node, ability);
		case AreaOfEffect.CIRCLE:
		case AreaOfEffect.CLEAVE:
			return FindCircleTargetTiles (node, ability);
		case AreaOfEffect.SINGLE:
		default:
			return FindSingleTargetTiles(node, ability);
		}

	}

	List<Node> FindSingleTargetTiles(Node node, BaseAbility ability) {
		List<Node> reachableTiles = findReachableTiles (node, ability.range, Walkable.Flying, -1).basic.Keys.ToList();
		if (ability.CanTargetSelf) {
			reachableTiles.Insert (0, node);
		}
		//TODO check to see if the tile is in line of sight
		return reachableTiles;
	}

	List<Node> FindCircleTargetTiles(Node node, BaseAbility ability) {
		List<Node> reachableTiles = findReachableTiles (node, ability.range, Walkable.Flying, -1).basic.Keys.ToList();
		if (ability.CanTargetSelf) {
			reachableTiles.Insert (0, node);
		}
		return reachableTiles;
	}

	public List<Node> FindAOEHitTiles(Node node, BaseAbility ability) {
		List<Node> targetTiles = findReachableTiles (node, ability.aoeRange, Walkable.Flying, -1).basic.Keys.ToList();
		targetTiles.Insert (0, node);
		return targetTiles;
	}

	public List<Node> FindCleaveTargetTiles(Node node, BaseAbility ability, Node start) {
		//TODO write a smarter way of doing this
		bool attackingHorizontally = start.x != node.x;
		List<Node> targetTiles = findReachableTiles (node, ability.aoeRange, Walkable.Flying, -1).basic.Keys.ToList();
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
		
}
