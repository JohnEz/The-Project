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
    public string type;
    public bool isAllied;

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

    public int tileWidth;
    public string type;
    public float version;
    public int width;
}

public struct MapData {
    public string name;
    public int width;
    public int height;
    public WalkableLevel[] walkableData;
    public LineOfSight[] lineOfSightData;
    public int[] roomData;
    public List<SpawnLocation> spawnLocations;
}

public class LevelLoaderJson : MonoBehaviour {
    private TiledMap loadedData;
    public MapData loadedLevel;

    public LevelObject defaultLevel;

    // Use this for initialization
    private void Start() {
    }

    public void Initialise() {
        LoadLevel();
    }

    public void LoadLevel() {
        if (GameDetails.Level == null) {
            GameDetails.Level = defaultLevel;
        }

        LoadTiledData();

        if (loadedData == null) {
            Debug.LogError("Error loading level");
            return;
        }

        loadedLevel.name = GameDetails.Level.fileName;
        loadedLevel.width = loadedData.width;
        loadedLevel.height = loadedData.height;

        loadedLevel.walkableData = CreateWalkableMap();
        loadedLevel.lineOfSightData = CreateLineOfSightMap();
        loadedLevel.spawnLocations = CreateSpawnLocations();
    }

    public WalkableLevel[] CreateWalkableMap() {
        WalkableLevel[] walkableArray = new WalkableLevel[loadedData.height * loadedData.width];

        Layer walkableLayer = loadedData.layers.Find(layer => layer.name.Equals("Walkable"));

        if (walkableLayer == null) {
            Debug.LogError("No walkable layer found");
        } else {
            for (int i = 0; i < walkableLayer.data.Length; i++) {
                switch (walkableLayer.data[i]) {
                    default:
                    case 2:
                        walkableArray[i] = WalkableLevel.Walkable;
                        break;

                    case 4:
                        walkableArray[i] = WalkableLevel.Flying;
                        break;

                    case 6:
                        walkableArray[i] = WalkableLevel.Impassable;
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

    public List<SpawnLocation> CreateSpawnLocations() {
        List<SpawnLocation> spawnLocations = new List<SpawnLocation>();

        Layer spawnLocationLayer = loadedData.layers.Find(layer => layer.name.Equals("SpawnLocations"));

        if (spawnLocationLayer == null) {
            Debug.LogWarning("No spawn location layer");
        } else {
            foreach (SpawnLocation spawnLocation in spawnLocationLayer.objects) {
                spawnLocation.x = Mathf.FloorToInt(spawnLocation.x / 128);
                spawnLocation.y = Mathf.FloorToInt(spawnLocation.y / 128);

                spawnLocation.isAllied = spawnLocation.type.Equals("ally");

                spawnLocations.Add(spawnLocation);
            }
        }

        return spawnLocations;
    }

    public void LoadTiledData() {
        // Path.Combine combines strings into a file path
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string filePath = Path.Combine(Application.streamingAssetsPath, GameDetails.Level.fileName + ".json");

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