using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Layer {
    public string name;
    public int[] data;
    public SpawnLocation[] objects;
}

public class TileSet {
}

[System.Serializable]
public class SpawnLocation {
    public string name;
    public int x, y;

    public override string ToString() {
        return string.Format("[ x: {0}, y: {1}, unit: {2} ]", x, y, name);
    }
}

public class TiledMap {
    public int height;
    public bool infinite;

    public List<Layer> layers;

    public int nextLayerId;
    public int nextObjectId;
    public string orientation;
    public string renderOrder;
    public string tiledVersion;
    public int tileHeight;

    //public TileSet[] tileSets;

    public int tileWidth;
    public string type;
    public float version;
    public int width;
}

public struct MapData {
    public string name;
    public int width;
    public int height;
    public Walkable[] walkableData;
    public LineOfSight[] lineOfSightData;
    public int[] roomData;
    public List<SpawnLocation> spawnLocations;
}

public class LevelLoaderJson : MonoBehaviour {
    private TiledMap loadedData;
    public MapData loadedLevel;

    public Sprite backdrop;

    // Use this for initialization
    private void Start() {
    }

    public void Initialise() {
        LoadLevel();
    }

    public void LoadLevel() {
        LoadTiledData();

        if (loadedData == null) {
            Debug.LogError("Error loading level");
            return;
        }

        loadedLevel.name = GameDetails.MapName;
        loadedLevel.width = loadedData.width;
        loadedLevel.height = loadedData.height;

        loadedLevel.walkableData = CreateWalkableMap();
        loadedLevel.lineOfSightData = CreateLineOfSightMap();
        loadedLevel.roomData = CreateRoomMap();
        loadedLevel.spawnLocations = CreateSpawnLocations();
    }

    public Walkable[] CreateWalkableMap() {
        Walkable[] walkableArray = new Walkable[loadedData.height * loadedData.width];

        Layer walkableLayer = loadedData.layers.Find(layer => layer.name.Equals("Walkable"));

        if (walkableLayer == null) {
            Debug.LogError("No walkable layer found");
        } else {
            for (int i = 0; i < walkableLayer.data.Length; i++) {
                switch (walkableLayer.data[i]) {
                    default:
                    case 2:
                        walkableArray[i] = Walkable.Walkable;
                        break;

                    case 4:
                        walkableArray[i] = Walkable.Flying;
                        break;

                    case 6:
                        walkableArray[i] = Walkable.Impassable;
                        break;
                }
            }
        }

        return walkableArray;
    }

    public LineOfSight[] CreateLineOfSightMap() {
        LineOfSight[] losArray = new LineOfSight[loadedData.height * loadedData.width];

        Layer losLayer = loadedData.layers.Find(layer => layer.name.Equals("LineOfSight"));

        if (losLayer == null) {
            Debug.LogError("No line of sight layer found");
        } else {
            for (int i = 0; i < losLayer.data.Length; i++) {
                //TODO change to switch with more values
                losArray[i] = losLayer.data[i] > 0 ? LineOfSight.Blocked : LineOfSight.Full;
            }
        }
        return losArray;
    }

    public int[] CreateRoomMap() {
        int[] roomArray = new int[loadedData.height * loadedData.width];

        Layer roomLayer = loadedData.layers.Find(layer => layer.name.Equals("Rooms"));

        if (roomLayer == null) {
            Debug.LogError("No room layer found");
        } else {
            //TODO potentially need some mapping here but its fine for now
            roomArray = roomLayer.data;
        }

        return roomArray;
    }

    public List<SpawnLocation> CreateSpawnLocations() {
        List<SpawnLocation> spawnLocations = new List<SpawnLocation>();

        Layer spawnLocationLayer = loadedData.layers.Find(layer => layer.name.Equals("SpawnLocations"));

        if (spawnLocationLayer == null) {
            Debug.LogWarning("No spawn location layer");
        } else {
            foreach (SpawnLocation spawnLocation in spawnLocationLayer.objects) {
                spawnLocation.x = Mathf.FloorToInt(spawnLocation.x / TileMap.TILE_SIZE);
                spawnLocation.y = Mathf.FloorToInt(spawnLocation.y / TileMap.TILE_SIZE);

                spawnLocations.Add(spawnLocation);
            }
        }

        return spawnLocations;
    }

    public void LoadTiledData() {
        // Path.Combine combines strings into a file path
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string filePath = Path.Combine(Application.streamingAssetsPath, GameDetails.MapName + ".json");

        if (File.Exists(filePath)) {
            // Read the json from the file into a string
            string dataAsJson = File.ReadAllText(filePath);
            // Pass the json to JsonUtility, and tell it to create a GameData object from it
            loadedData = JsonUtility.FromJson<TiledMap>(dataAsJson);
        } else {
            Debug.LogError("Cannot load game data!");
        }
    }
}