﻿using UnityEngine;
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

	public Dictionary<Node, float> findReachableTiles(Node startNode, float distance, Walkable walkingType, int team) {
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
					
					if (neighbour.node != startNode && isTileWalkable(node, neighbour.node, walkingType, team)) {
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

		costToNode = costToNode.Where (item => item.Key.myUnit == null).ToDictionary(item => item.Key, item => item.Value);

		return costToNode;
	}

	public MovementPath getPathFromTile (Node endNode) {
		Node currentNode = endNode;
		MovementPath newPath = new MovementPath ();
		newPath.path = new List<Node> ();

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

	public bool isTileWalkable(Node startNode, Node endNode, Walkable walkingType, int team) {
		bool passed = true;

		if (endNode.walkable > walkingType) {
			passed = false;
		}

		if (endNode.myUnit != null && endNode.myUnit.myTeam != team) {
			passed = false;
		}

		int levelDifference = Math.Abs(startNode.level - endNode.level);

		int maxDifference = (int)walkingType;

		if (levelDifference > maxDifference+1) {
			passed = false;
		}

		return passed;
	}


		
}
