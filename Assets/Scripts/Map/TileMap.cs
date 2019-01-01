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

	public GameObject basicNodePrefab;
	public GameObject tileTemplatePrefab;
    public GameObject doorPrefab;

	public Pathfinder pathfinder;

	float tileSize = 128;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialise() {
		pathfinder = GetComponent<Pathfinder> ();
		pathfinder.Initialise ();

        LevelLoaderJson lLoaderJson = GetComponent<LevelLoaderJson>();
        lLoaderJson.Initialise();
        GenerateMapJson(lLoaderJson.loadedLevel);
        CalculateNeighbours();

        //LevelLoader lLoader = GetComponent<LevelLoader> ();
		//lLoader.Initialise ();
		//GenerateMap (lLoader.GetLevel(0));
	}

    void GenerateMapJson(MapData data) {
        tiles = new Node[data.width * data.height];

        mapWidth = data.width;
        mapHeight = data.height;

        //Generate empty nodes
        for (int i = 0; i < tiles.Length; ++i) {
            Quaternion rot = basicNodePrefab.transform.rotation;
            int x = i % data.width;
            int y = i / data.width;

            Vector3 pos = new Vector3(x * tileSize, -y * tileSize, 0);

            GameObject baseNode = Instantiate(basicNodePrefab, pos, rot);
            baseNode.transform.parent = this.transform;

            baseNode.GetComponentInChildren<TileHighlighter>().Initialise();

            tiles[i] = baseNode.GetComponent<Node>();
            tiles[i].x = x;
            tiles[i].y = y;
            tiles[i].walkable = data.walkableData[i];
            tiles[i].lineOfSight = data.lineOfSightData[i];
            tiles[i].room = data.roomData[i];
            tiles[i].height = 0;
            tiles[i].moveCost = 1;
        }
    }


	void GenerateMap(Level level) {
		tiles = new Node[level.maxSizeX * level.maxSizeY];
		bool[] overriden = new bool[level.maxSizeX * level.maxSizeY];

		mapWidth = level.maxSizeX;
		mapHeight = level.maxSizeY;


		for (int j = 0; j < level.layers.Length; ++j) {
			for (int i = 0; i < level.layers[j].tiles.Length; ++i) {
			
				Quaternion rot = basicNodePrefab.transform.rotation;
				int x = i % level.maxSizeY;
				int y = i / level.maxSizeX;
				Vector3 pos = new Vector3 (x * tileSize, -y * tileSize, 0);

				int id = level.layers [j].tiles [i] - 1;

				if (tiles [i] == null) {
					
					GameObject baseNode = Instantiate (basicNodePrefab, pos, rot);
					baseNode.transform.parent = this.transform;

					baseNode.GetComponentInChildren<TileHighlighter> ().Initialise ();

					tiles [i] = baseNode.GetComponent<Node> ();
					tiles [i].x = x;
					tiles [i].y = y;
					tiles [i].height = level.heightMap [i];
				}

				if (id >= 0) {
					pos = Vector3.zero;

					GameObject visual = (GameObject)Instantiate (tileTemplatePrefab, pos, rot);

					//visual.GetComponent<SpriteRenderer> ().sprite = level.textures [id];

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

    void AddNeighbour(Node start, int dirX, int dirY) {
        Neighbour neighbour = new Neighbour();
        neighbour.node = getNode(start.x+dirX, start.y+dirY);
        neighbour.direction = new Vector2(dirX, -dirY);

        bool bothRoomsInMap = start.room != 0 && neighbour.node.room != 0;
        bool differentRooms = start.room != neighbour.node.room;
        neighbour.hasDoor = bothRoomsInMap && differentRooms;

        start.neighbours.Add(neighbour);
    }

	void CalculateNeighbours() {

		int x = 0;
		int y = 0;

		//find neighbours
		for (int i = 0; i < tiles.Length; ++i) {

			x = i % mapWidth;
			y = i / mapWidth;

            Node node = getNode(x, y);
            node.neighbours = new List<Neighbour>();

			//set all neighbours
			if (x > 0) {
                AddNeighbour(node, -1, 0);
			}
			if (x < mapWidth-1) {
                AddNeighbour(node, 1, 0);
			}
			if (y > 0) {
                AddNeighbour(node, 0, -1);
			}
			if (y < mapHeight-1) {
                AddNeighbour(node, 0, 1);
			}

		}
	}

	public Node getNode(int x, int y) {
		return tiles [y * mapWidth + x];
	}

	public Vector3 getPositionOfNode(Node targetNode) {
		return targetNode.transform.position;
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
