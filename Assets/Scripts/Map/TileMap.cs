using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

[System.Serializable]
public class PrefabDictionaryEntry
{
	public int key;
	public GameObject value;
}

public class TileMap : MonoBehaviour {


	int mapWidth;
	int mapHeight;
	Node[] tiles;

	public GameObject basicNode;
	public GameObject tileTemplate;

	public Pathfinder pathfinder;
	public HighlightManager highlighter;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialise() {
		pathfinder = GetComponent<Pathfinder> ();
		highlighter = GetComponent<HighlightManager> ();
		LevelLoader lLoader = GetComponent<LevelLoader> ();
		lLoader.Initialise ();
		GenerateMap (lLoader.GetLevel(0));
	}

	void GenerateMap(Level level) {
		tiles = new Node[level.maxSizeX * level.maxSizeY];
		bool[] overriden = new bool[level.maxSizeX * level.maxSizeY];

		mapWidth = level.maxSizeX;
		mapHeight = level.maxSizeY;


		for (int j = 0; j < level.layers.Length; ++j) {
			for (int i = 0; i < level.layers[j].tiles.Length; ++i) {
			
				Quaternion rot = basicNode.transform.rotation;
				int x = i % level.maxSizeY;
				int y = i / level.maxSizeX;
				Vector3 pos = new Vector3 (x, -y, 0);

				int id = level.layers [j].tiles [i] - 1;

				if (tiles [i] == null) {
					
					GameObject baseNode = Instantiate (basicNode, pos, rot);
					baseNode.transform.parent = this.transform;

					baseNode.GetComponentInChildren<TileHighlighter> ().Initialise ();

					tiles [i] = baseNode.GetComponent<Node> ();
					tiles [i].x = x;
					tiles [i].y = y;
					tiles [i].level = level.heightMap [i];
				}

				if (id >= 0) {
					pos = Vector3.zero;

					GameObject visual = (GameObject)Instantiate (tileTemplate, pos, rot);

					visual.GetComponent<SpriteRenderer> ().sprite = level.textures [id];

					visual.transform.parent = tiles [i].transform;

					visual.transform.localPosition = Vector3.zero;

					visual.GetComponent<SpriteRenderer> ().sortingOrder = level.layers [j].depth;

					//this might not need to be a node if it just stores walkable
					visual.GetComponent<Node> ().walkable = level.tileInfo [id].walkable;

					//overriden is used for stairs, bridges etc.
					if (!overriden [i]) {
						if (level.tileInfo [id].overrideWalkability) {
							overriden [i] = true;
							tiles [i].walkable = visual.GetComponent<Node> ().walkable;
						} else if (tiles [i].walkable < visual.GetComponent<Node> ().walkable) {
							tiles [i].walkable = visual.GetComponent<Node> ().walkable;
						}
					}

					if (tiles [i].moveCost < visual.GetComponent<Node> ().moveCost) {
						tiles [i].moveCost = visual.GetComponent<Node> ().moveCost;
					}
				}
			}
		}
		CalculateNeighbours ();
	}

	void CalculateNeighbours() {

		int x = 0;
		int y = 0;

		//find neighbours
		for (int i = 0; i < tiles.Length; ++i) {

			x = i % mapWidth;
			y = i / mapHeight;

			getNode (x, y).neighbours = new List<Neighbour>();

			//set all neighbours
			if (x > 0) {
				Neighbour left;
				left.node = getNode(x-1, y);
				left.direction = new Vector2 (-1, 0);
				getNode(x, y).neighbours.Add(left);
			}
			if (x < mapWidth-1) {
				Neighbour right;
				right.node = getNode(x+1, y);
				right.direction = new Vector2 (1, 0);
				getNode(x, y).neighbours.Add(right);
			}
			if (y > 0) {
				Neighbour up;
				up.node = getNode(x, y-1);
				up.direction = new Vector2 (0, 1);
				getNode(x, y).neighbours.Add(up);
			}
			if (y < mapHeight-1) {
				Neighbour down;
				down.node = getNode(x, y+1);
				down.direction = new Vector2 (0, -1);
				getNode(x, y).neighbours.Add(down);
			}

		}
	}

	public Node getNode(int x, int y) {
		return tiles [y * mapWidth + x];
	}

	public Vector3 getPositionOfNode(int x, int y) {
		return getNode(x, y).transform.position;
	}

	public float getWidth() {
		return mapWidth;
	}

	public float getHeight() {
		return mapHeight;
	}

	public void resetTiles() {
		foreach (Node n in tiles) {
			n.Reset ();
		}
	}

	public Vector2 GetDirectionBetweenNodes(Node startNode, Node endNode) {
		Vector2 direction = new Vector2 (endNode.x - startNode.x, -(endNode.y - startNode.y));

		return direction.normalized;
	}

}
