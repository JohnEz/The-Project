using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System;

public struct Level {
	public string name;
	public int maxSizeX;
	public int maxSizeY;
	public MapLayer[] layers;
	public TileProperties[] tileInfo;
	public Sprite[] textures;
	public int[] heightMap;
}

public struct TileProperties {
	public Walkable walkable;
	public bool overrideWalkability;
}

public struct MapLayer {
	public int[] tiles;
	public int depth;
}

public class LevelLoader : MonoBehaviour {

    List<Level> levels;

    public TextAsset testLoadFile;

    // Use this for initialization
    void Start()
    {

    }

	public void Initialise () {
		levels = new List<Level>();
		loadLevel(testLoadFile);
	}

	public Level GetLevel(int i){
		return levels [i];
	}

	public int[] CreateHeightMap(int maxX, int maxY, XmlNodeList layer) {
		
		int[] heightMap = new int[maxX * maxY];
		foreach (XmlNode content in layer) {
			//if the xml node is the data
			if (content.Name.Equals ("data")) {
				int count = 0;
				//loop through all the tiles and get their ids
				foreach (XmlNode height in content) {
					heightMap [count] = Int32.Parse (height.Attributes.GetNamedItem ("gid").Value);
					count++;
				}
			}
		}
		return heightMap;
	}

	public MapLayer CreateMapLayer(int maxX, int maxY, XmlNodeList layer, int depth) {
		MapLayer newLayer = new MapLayer ();
		newLayer.depth = depth;
		newLayer.tiles = new int[maxX * maxY];

		foreach (XmlNode content in layer) {
			//if the xml node is the data
			if (content.Name.Equals ("data")) {
				int count = 0;
				//loop through all the tiles and get their ids
				foreach (XmlNode tile in content) {
					newLayer.tiles [count] = Int32.Parse (tile.Attributes.GetNamedItem ("gid").Value);
					count++;
				}
			}
		}
		return newLayer;
	}

    public void loadLevel(TextAsset loadedXml)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(loadedXml.text);
        XmlNodeList levelList = xmlDoc.GetElementsByTagName("layer");

		Level newLevel = new Level();
		int layer = 0;

		//for each place labeled "layer"
        foreach (XmlNode levelInfo in levelList)
        {
			XmlNodeList levelContent = levelInfo.ChildNodes;

			//get the attributes of the level
			if (layer == 0) {
				newLevel.layers = new MapLayer[levelList.Count-1];
				newLevel.maxSizeX = Int32.Parse (levelInfo.Attributes.GetNamedItem ("width").Value);
				newLevel.maxSizeY = Int32.Parse (levelInfo.Attributes.GetNamedItem ("height").Value);
			}

			string layerValue = levelInfo.Attributes.GetNamedItem ("name").Value;

			if (layerValue.Equals ("Height Map")) {
				newLevel.heightMap = CreateHeightMap (newLevel.maxSizeX, newLevel.maxSizeY, levelContent);
			} else {
				newLevel.layers [layer] = CreateMapLayer (newLevel.maxSizeX, newLevel.maxSizeY, levelContent, Int32.Parse (layerValue));
				layer++;
			}
        }

		newLevel.textures = LoadTextures ();
		newLevel.tileInfo = LoadTileInfo (newLevel.textures.Length);
		levels.Add (newLevel);
    }

	Sprite[] LoadTextures() {
		string textures = "Graphics/Tiles/tiles";
		return Resources.LoadAll<Sprite>(textures);
	}

	TileProperties[] LoadTileInfo(int length) {
		string infoPath = "XmlDocs/tileInfo";
		TextAsset infoDoc = (TextAsset) Resources.Load(infoPath);

		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(infoDoc.text);
		XmlNodeList tiles = xmlDoc.GetElementsByTagName("Tile");

		TileProperties[] tileInfos = new TileProperties[length];

		//for each place labeled "Tile"
		foreach (XmlNode tile in tiles) {
			string walkability = tile.Attributes.GetNamedItem ("walkable").Value;
			int id = Int32.Parse (tile.Attributes.GetNamedItem ("id").Value);
			XmlNode overridesWalkability = tile.Attributes.GetNamedItem ("override");
	
			Walkable walkable = Walkable.Walkable;
			bool overridesWalkable = false;

			if (overridesWalkability != null) {
				overridesWalkable = true;
			}

			switch (walkability) {
			case "0":
				walkable = Walkable.Walkable;
				break;
			case "1":
				walkable = Walkable.Flying;
				break;
			case "2":
				walkable = Walkable.Impassable;
				break;
			}

			tileInfos [id].walkable = walkable;
			tileInfos [id].overrideWalkability = overridesWalkable;
		}

		return tileInfos;
	}

}
