using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	public PrefabDictionaryEntry[] tilePrefabs;
	private Dictionary<int, GameObject> prefabs;
	public GameObject basicNode;

	// Use this for initialization
	void Start () {
		CreatePrefabDictionary ();

		LevelLoader lLoader = GetComponent<LevelLoader> ();
		lLoader.Initialise ();
		GenerateMap (lLoader.GetLevel(0));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void CreatePrefabDictionary() {
		prefabs = new Dictionary<int, GameObject> ();

		foreach (PrefabDictionaryEntry entry in tilePrefabs) {
			prefabs.Add (entry.key, entry.value);
		}
	}

	void GenerateMap(Level level) {
		tiles = new Node[level.maxSizeX * level.maxSizeY];

		mapWidth = level.maxSizeX;
		mapHeight = level.maxSizeY;


		for (int j = 0; j < level.layers.Length; ++j) {
			for (int i = 0; i < level.layers[j].tiles.Length; ++i) {
			
				if (tiles [i] == null) {
					Quaternion rot = basicNode.transform.rotation;
					int x = i % level.maxSizeY;
					int y = i / level.maxSizeX;
					Vector3 pos = new Vector3 (x, -y, 0);
					GameObject baseNode = (GameObject)Instantiate (basicNode, pos, rot);

					baseNode.transform.parent = this.transform;

					tiles [i] = baseNode.GetComponent<Node> ();
					tiles [i].x = x;
					tiles [i].y = y;
				}

				if (prefabs.ContainsKey(level.layers [j].tiles [i])) {
					
					GameObject tile = prefabs [level.layers [j].tiles [i]];

					Quaternion rot = tile.transform.rotation;
					Vector3 pos = Vector3.zero;

					GameObject visual = (GameObject)Instantiate (tile, pos, rot);

					visual.transform.parent = tiles[i].transform;

					visual.transform.localPosition = Vector3.zero;

					visual.GetComponent<SpriteRenderer> ().sortingOrder = level.layers [j].depth;

					if (tiles [i].walkable < visual.GetComponent<Node> ().walkable) {
						tiles [i].walkable = visual.GetComponent<Node> ().walkable;
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
				up.direction = new Vector2 (0, -1);
				getNode(x, y).neighbours.Add(up);
			}
			if (y < mapHeight-1) {
				Neighbour down;
				down.node = getNode(x, y+1);
				down.direction = new Vector2 (0, 1);
				getNode(x, y).neighbours.Add(down);
			}

		}
	}

	public Node getNode(int x, int y) {
		return tiles [y * mapWidth + x];
	}
}
