using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public struct MovementPath {
	public List<Node> path;
	public int movementCost;
}

public class Pathfinder : MonoBehaviour {

	TileMap map;

	// Use this for initialization
	void Start () {
		map = GetComponent<TileMap> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Dictionary<Node, float> findReachableTiles(Node startNode, float distance, Walkable walkingType, int faction) {
		List<Node> openList = new List<Node> ();
		List<Node> currentList;
		Dictionary<Node, float> costToNode = new Dictionary<Node, float> ();
		map.resetTiles ();
		bool changed = true;
		openList.Add (startNode);

		while (changed) {
			changed = false;
			currentList = openList;
			openList = new List<Node> ();

			foreach (Node node in currentList) {

				foreach (Neighbour neighbour in node.neighbours) {
					
					if (neighbour.node != startNode && isTileWalkable(node, neighbour.node, walkingType, faction)) {
						float totalCost = node.cost + neighbour.node.moveCost;

						if (totalCost <= distance) {
							if (!costToNode.ContainsKey (neighbour.node) || costToNode[neighbour.node] > totalCost) {
								neighbour.node.previous = new Neighbour();
								neighbour.node.previous.node = node;
								neighbour.node.previous.direction = neighbour.direction;
								neighbour.node.cost = totalCost;
								costToNode [neighbour.node] = totalCost;
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
			costToNode = costToNode.Where (item => item.Key.myUnit == null).ToDictionary (item => item.Key, item => item.Value);
		}

		return costToNode;
	}

	public MovementPath FindPath(Node source, Node target, Walkable walkingType, int faction) {
		map.resetTiles ();

		List<Node> openList = new List<Node> ();
		List<Node> closedList = new List<Node> ();
		MovementPath path = new MovementPath ();

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
			closedList.Add (currentNode);

			foreach (Neighbour neighbour in currentNode.neighbours) {
				if (neighbour.node != source && (isTileWalkable(currentNode, neighbour.node, walkingType, faction) || neighbour.node == target)) {
					float totalCost = currentNode.cost + neighbour.node.moveCost;

					if (!closedList.Contains(neighbour.node) || neighbour.node.cost > totalCost) {
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

	public List<Node> FindAttackableTiles(UnitController caster, BaseAbility ability) {
		//TODO switch state on if its aoe, cleave, single target etc
		return FindSingleTargetTiles(caster, ability);
	}

	List<Node> FindSingleTargetTiles(UnitController caster, BaseAbility ability) {
		List<Node> reachableTiles = findReachableTiles (caster.myNode, ability.range, Walkable.Flying, -1).Keys.ToList();

		//TODO check to see if the tile is in line of sight
		return reachableTiles;
	}


		
}
