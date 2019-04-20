using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using TMPro;
using System.Linq;

public class LevelSelect : MonoBehaviour {
    public TMP_Dropdown dropdown;
    private List<string> maps;

    public void Awake() {
        maps = LoadMaps();
        dropdown.AddOptions(maps);
        SelectMap(0);
    }

    public List<string> LoadMaps() {
        List<string> filePaths = new List<string>(Directory.GetFiles(Application.streamingAssetsPath, "*.json",
                                         SearchOption.AllDirectories));

        return filePaths.Select((filepath) => {
            return filepath.Replace(Application.streamingAssetsPath + "\\", "").Replace(".json", "");
        }).ToList();
    }

    public void SelectMap(int index) {
        GameDetails.MapName = maps[index];
        Debug.Log("Set map to " + GameDetails.MapName);
    }
}