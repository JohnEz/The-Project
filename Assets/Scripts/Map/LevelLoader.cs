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
				newLevel.layers = new MapLayer[levelList.Count];
				newLevel.maxSizeX = Int32.Parse (levelInfo.Attributes.GetNamedItem ("width").Value);
				newLevel.maxSizeY = Int32.Parse (levelInfo.Attributes.GetNamedItem ("height").Value);
			}

			newLevel.layers [layer].depth = Int32.Parse (levelInfo.Attributes.GetNamedItem ("name").Value);
			newLevel.layers[layer].tiles = new int[newLevel.maxSizeX * newLevel.maxSizeY];

			foreach (XmlNode content in levelContent) {

				//if the xml node is the data
				if (content.Name.Equals("data")) {
					int count = 0;
					//loop through all the tiles and get their ids
					foreach(XmlNode tile in content) {
						newLevel.layers[layer].tiles[count] = Int32.Parse (tile.Attributes.GetNamedItem ("gid").Value);
						count++;
					}

				}

            }

			levels.Add (newLevel);
			layer++;
        }

    }


}
