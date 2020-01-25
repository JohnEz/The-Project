using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PrefabDictionaryEntry {
    public int key;
    public GameObject value;
}

public class Room {
    public int id;
    public bool isActive;
    public List<Node> nodes;

    public Room(int _id) {
        id = _id;
        isActive = false;
        nodes = new List<Node>();
    }
}

public class TileMap : MonoBehaviour {
    public const float TILE_SIZE = 12.8f;

    public static TileMap instance;

    private int mapWidth;
    private int mapHeight;
    private Node[] tilesSmall;
    private NodeCollection[] tilesMedium;
    private Tile[] tilesLarge;
    private Dictionary<int, Room> rooms;

    public GameObject basicNodePrefab;
    public GameObject tileTemplatePrefab;
    public GameObject doorPrefab;

    public Pathfinder pathfinder;

    public bool isMapLoaded = false;

    public List<SpawnLocation> spawnLocations;

    private void Awake() {
        instance = this;
    }

    public void Initialise() {
        rooms = new Dictionary<int, Room>();
        pathfinder = GetComponent<Pathfinder>();
        pathfinder.Initialise();

        spawnLocations = new List<SpawnLocation>();

        LevelLoaderJson lLoaderJson = GetComponent<LevelLoaderJson>();
        lLoaderJson.Initialise();
        Instantiate(GameDetails.Level.mapObject);

        GenerateSmallTiles(lLoaderJson.loadedLevel);
        GenerateMediumTiles();

        isMapLoaded = true;
    }

    public List<Node> GetNodes() {
        return new List<Node>(tilesSmall);
    }

    public Node GetNode(Vector2 pos) {
        return GetNode((int)pos.x, (int)pos.y);
    }

    public Node GetNode(int x, int y) {
        return tilesSmall[y * mapWidth + x];
    }

    private static Tile GetTile(Tile[] tiles, int width, int x, int y) {
        int index = y * width + x;
        if (index > tiles.Length - 1 || index < 0) {
            Debug.Log("x:" + x);
            Debug.Log("y:" + y);
            Debug.Log("tiles.length:" + tiles.Length);
            Debug.Log("width:" + width);
        }

        return tiles[y * width + x];
    }

    public static Vector3 getPositionOfNodes(NodeCollection targetNodes) {
        Vector3 sumOfNodes = Vector3.zero;

        targetNodes.Nodes.ForEach((Node node) => { sumOfNodes += getPositionOfNode(node); });

        return sumOfNodes;
    }

    public static Vector3 getPositionOfNode(Node targetNode) {
        return targetNode.transform.position;
    }

    public Vector3 getPositionOfNode(int x, int y) {
        return getPositionOfNode(GetNode(x, y));
    }

    public float getWidth() {
        return mapWidth;
    }

    public float getHeight() {
        return mapHeight;
    }

    public void resetTiles() {
        foreach (Node n in tilesSmall) {
            n.Reset();
        }
    }

    public bool IsRoomActive(int roomId) {
        return rooms[roomId].isActive;
    }

    public void ActivateRoom(int roomId) {
        rooms[roomId].isActive = true;
        rooms[roomId].nodes.ForEach(node => {
            node.Activate();
        });
    }

    public Vector2 GetDirectionBetweenNodes(Node startNode, Node endNode) {
        Vector2 direction = new Vector2(endNode.x - startNode.x, -(endNode.y - startNode.y));

        return direction.normalized;
    }

    private void GenerateSmallTiles(MapData data) {
        mapWidth = data.width;
        mapHeight = data.height;
        tilesSmall = new Node[mapWidth * mapHeight];

        spawnLocations = data.spawnLocations;

        float halfTileSize = TILE_SIZE / 2;

        //Generate empty nodes
        for (int i = 0; i < tilesSmall.Length; ++i) {
            Quaternion rot = basicNodePrefab.transform.rotation;
            int x = i % mapWidth;
            int y = i / mapWidth;

            Vector3 pos = new Vector3((x * TILE_SIZE) + halfTileSize, 0, (-y * TILE_SIZE) - halfTileSize);

            GameObject baseNode = Instantiate(basicNodePrefab, pos, rot);
            baseNode.transform.parent = this.transform;

            baseNode.GetComponentInChildren<TileHighlighter>().Initialise();

            tilesSmall[i] = baseNode.GetComponent<Node>();
            tilesSmall[i].x = x;
            tilesSmall[i].y = y;
            tilesSmall[i].Walkable = data.walkableData[i];
            tilesSmall[i].lineOfSight = data.lineOfSightData[i];
            tilesSmall[i].room = data.roomData[i];
            tilesSmall[i].height = 0;
            tilesSmall[i].MoveCost = 1;

            AddNodeToRoom(data.roomData[i], tilesSmall[i]);
        }

        CalculateNeighbours(tilesSmall, mapWidth, mapHeight);
    }

    private void GenerateMediumTiles() {
        int width = mapWidth - 1;
        int height = mapHeight - 1;

        tilesMedium = new NodeCollection[width * height];

        //Generate empty tiles
        for (int i = 0; i < tilesMedium.Length; ++i) {
            Quaternion rot = basicNodePrefab.transform.rotation;

            //xy = top left node of tile
            int x = i % width;
            int y = i / width;

            Vector3 pos = new Vector3(((x + 1) * TILE_SIZE), 0, ((-y - 1) * TILE_SIZE));
            GameObject baseTile = Instantiate(tileTemplatePrefab, pos, rot);
            baseTile.transform.parent = this.transform;

            tilesMedium[i] = baseTile.GetComponent<NodeCollection>();

            // TODO do i need a check here to make sure it doesnt go out of bounds?
            tilesMedium[i].x = x;
            tilesMedium[i].y = y;
            tilesMedium[i].Add(GetNode(x, y));
            tilesMedium[i].Add(GetNode(x + 1, y));
            tilesMedium[i].Add(GetNode(x, y + 1));
            tilesMedium[i].Add(GetNode(x + 1, y + 1));
        }

        CalculateNeighbours(tilesMedium, width, height);
    }

    private void AddNeighbour(Tile[] tiles, Tile startTile, int width, int dirX, int dirY) {
        int x = startTile.x + dirX;
        int y = startTile.y + dirY;
        Tile endTile = GetTile(tiles, width, x, y);

        Neighbour exisitingNeighbour = endTile.neighbours != null ? endTile.FindNeighbourTo(startTile) : null;
        // check to see if the end node has already created a neighbour
        if (exisitingNeighbour != null) {
            startTile.neighbours.Add(exisitingNeighbour);
            return;
        }

        Neighbour neighbour = new Neighbour(startTile, endTile);

        startTile.neighbours.Add(neighbour);
    }

    private void CalculateNeighbours(Tile[] tiles, int width, int height) {
        //find neighbours
        for (int i = 0; i < tiles.Length; ++i) {
            int x = i % width;
            int y = i / width;

            Tile tile = GetTile(tiles, width, x, y);
            tile.neighbours = new List<Neighbour>();

            //set all neighbours
            if (x > 0) {
                AddNeighbour(tiles, tile, width, -1, 0);
                //if (y > 0) {
                //    AddNeighbour(node, -1, -1);
                //}
                //if (y < mapHeight - 1) {
                //    AddNeighbour(node, -1, 1);
                //}
            }
            if (x < width - 1) {
                AddNeighbour(tiles, tile, width, 1, 0);
                //if (y > 0) {
                //    AddNeighbour(node, 1, -1);
                //}
                //if (y < mapHeight - 1) {
                //    AddNeighbour(node, 1, 1);
                //}
            }
            if (y > 0) {
                AddNeighbour(tiles, tile, width, 0, -1);
            }
            if (y < height - 1) {
                AddNeighbour(tiles, tile, width, 0, 1);
            }
        }
    }

    public void AddNodeToRoom(int roomId, Node node) {
        if (!rooms.ContainsKey(roomId)) {
            rooms.Add(roomId, new Room(roomId));
        } else if (rooms[roomId].nodes.Contains(node)) {
            Debug.Log("Node already existed in room!");
            return;
        }

        rooms[roomId].nodes.Add(node);
    }
}