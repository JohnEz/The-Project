using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class Layer {
    public string name;
    public int[] data;
}

public class TileSet {

}

public class TiledMap {
    public int height;
    public bool infinite;

    public Layer[] layers;

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
    public Walkable[] walkableList;
}

public class LevelLoaderJson : MonoBehaviour {

    TiledMap loadedData;
    public MapData loadedLevel;

    public string levelName;

    public Sprite backdrop;

    // Use this for initialization
    void Start() {

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

        loadedLevel.name = levelName;
        loadedLevel.width = loadedData.width;
        loadedLevel.height = loadedData.height;

        loadedLevel.walkableList = CreateWalkableMap();
    }

    public Walkable[] CreateWalkableMap() {
        Walkable[] walkableArray = new Walkable[loadedData.height * loadedData.width];

        Layer walkableLayer = null;

        foreach(Layer layer in loadedData.layers) {
            if (layer.name.Equals("Walkable")) {
                walkableLayer = layer;
            }
        }

        if (walkableLayer == null) {
            Debug.LogError("No walkable layer found");
        }

        for (int i=0; i < walkableLayer.data.Length; i++) {
            switch (walkableLayer.data[i]) {
                default:
                case 2: walkableArray[i] = Walkable.Walkable;
                    break;
                case 4: walkableArray[i] = Walkable.Flying;
                    break;
                case 6: walkableArray[i] = Walkable.Impassable;
                    break;
            }
        }

        return walkableArray;
    }

    public void LoadTiledData() {

        // Path.Combine combines strings into a file path
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string filePath = Path.Combine(Application.streamingAssetsPath, levelName + ".json");

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
