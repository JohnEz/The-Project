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
    private Node[] tiles;
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

        GenerateMap(lLoaderJson.loadedLevel);
        CalculateNeighbours();

        isMapLoaded = true;
    }

    public List<Node> GetNodes() {
        return new List<Node>(tiles);
    }

    public Node GetNode(Vector2 pos) {
        return GetNode((int)pos.x, (int)pos.y);
    }

    public Node GetNode(int x, int y) {
        return tiles[y * mapWidth + x];
    }

    public Vector3 getPositionOfNode(Node targetNode) {
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
        foreach (Node n in tiles) {
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

    private void GenerateMap(MapData data) {
        mapWidth = data.width;
        mapHeight = data.height;
        tiles = new Node[mapWidth * mapHeight];

        spawnLocations = data.spawnLocations;

        float halfTileSize = TILE_SIZE / 2;

        //Generate empty nodes
        for (int i = 0; i < tiles.Length; ++i) {
            Quaternion rot = basicNodePrefab.transform.rotation;
            int x = i % data.width;
            int y = i / data.width;

            Vector3 pos = new Vector3((x * TILE_SIZE) + halfTileSize, 0, (-y * TILE_SIZE) - halfTileSize);

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

            AddNodeToRoom(data.roomData[i], tiles[i]);
        }
    }

    private void AddNeighbour(Node startNode, int dirX, int dirY) {
        Node endNode = GetNode(startNode.x + dirX, startNode.y + dirY);

        Neighbour exisitingNeighbour = endNode.neighbours != null ? endNode.FindNeighbourTo(startNode) : null;
        // check to see if the end node has already created a neighbour
        if (exisitingNeighbour != null) {
            startNode.neighbours.Add(exisitingNeighbour);
            return;
        }

        Neighbour neighbour = new Neighbour(startNode, endNode);

        bool bothRoomsInMap = startNode.room != 0 && endNode.room != 0;
        bool differentRooms = startNode.room != endNode.room;
        if (bothRoomsInMap && differentRooms) {
            neighbour.AddDoor(doorPrefab);
        }

        startNode.neighbours.Add(neighbour);
    }

    private void CalculateNeighbours() {
        int x = 0;
        int y = 0;

        //find neighbours
        for (int i = 0; i < tiles.Length; ++i) {
            x = i % mapWidth;
            y = i / mapWidth;

            Node node = GetNode(x, y);
            node.neighbours = new List<Neighbour>();

            //set all neighbours
            if (x > 0) {
                AddNeighbour(node, -1, 0);
                //if (y > 0) {
                //    AddNeighbour(node, -1, -1);
                //}
                //if (y < mapHeight - 1) {
                //    AddNeighbour(node, -1, 1);
                //}
            }
            if (x < mapWidth - 1) {
                AddNeighbour(node, 1, 0);
                //if (y > 0) {
                //    AddNeighbour(node, 1, -1);
                //}
                //if (y < mapHeight - 1) {
                //    AddNeighbour(node, 1, 1);
                //}
            }
            if (y > 0) {
                AddNeighbour(node, 0, -1);
            }
            if (y < mapHeight - 1) {
                AddNeighbour(node, 0, 1);
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