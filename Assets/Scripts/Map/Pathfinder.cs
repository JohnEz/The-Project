using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinder : MonoBehaviour {

	TileMap map;

	// Use this for initialization
	void Start () {
		map = GetComponent<TileMap> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Dictionary<Node, float> findReachableTiles(Node startNode, float distance, Walkable walkingType) {
		List<Node> openList = new List<Node> ();
		List<Node> currentList;
		Dictionary<Node, float> costToNode = new Dictionary<Node, float> ();
		bool changed = true;
		openList.Add (startNode);



		while (changed) {
			changed = false;
			currentList = openList;
			openList = new List<Node> ();

			foreach (Node node in currentList) {

				foreach (Neighbour neighbour in node.neighbours) {
					
					if (neighbour.node != startNode && isTileWalkable(neighbour.node, walkingType)) {
						float totalCost = node.cost + neighbour.node.moveCost;

						if (totalCost <= distance) {
							if (!costToNode.ContainsKey (neighbour.node) || costToNode[neighbour.node] > totalCost) {
								neighbour.node.previous = new Neighbour();
								neighbour.node.previous.node = node;
								neighbour.node.previous.direction = neighbour.direction * -1;
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

		return costToNode;
	}

	public bool isTileWalkable(int x, int y, Walkable walkingType) {
		return isTileWalkable (map.getNode (x, y), walkingType);
	}

	public bool isTileWalkable(Node node, Walkable walkingType) {
		return node.walkable <= walkingType;
	}


		
}
