using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using TMPro;
using System.Linq;
using DuloGames.UI;

public class LevelSelect : MonoBehaviour {
    public UISelectField dropdown;
    private List<string> maps;

    public void Awake() {
        maps = LoadMaps();
        foreach (string map in maps) {
            dropdown.AddOption(map);
        }

        GameDetails.MapName = maps[0];
        dropdown.SelectOption(maps[0]);
    }

    protected void OnEnable() {
        if (dropdown == null)
            return;

        dropdown.onChange.AddListener(OnSelectedMapOption);
    }

    protected void OnDisable() {
        if (dropdown == null)
            return;

        dropdown.onChange.RemoveListener(OnSelectedMapOption);
    }

    protected void OnSelectedMapOption(int index, string option) {
        GameDetails.MapName = maps[index];
    }

    public List<string> LoadMaps() {
        List<string> filePaths = new List<string>(Directory.GetFiles(Application.streamingAssetsPath, "*.json",
                                         SearchOption.AllDirectories));

        return filePaths.Select((filepath) => {
            return filepath.Replace(Application.streamingAssetsPath + "\\", "").Replace(".json", "");
        }).ToList();
    }
}