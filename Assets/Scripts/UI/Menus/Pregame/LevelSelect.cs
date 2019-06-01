using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using TMPro;
using System.Linq;
using DuloGames.UI;

public class LevelSelect : MonoBehaviour {
    public UISelectField dropdown;
    private List<LevelObject> levels;

    public void Awake() {
        levels = LoadLevels();

        if (!Debug.isDebugBuild) {
            LevelObject devLevel = levels.Find(level => level.index == -1);
            levels.Remove(devLevel);
        }

        levels.Sort((l, r) => { return l.index - r.index; });
        foreach (LevelObject level in levels) {
            dropdown.AddOption(level.levelName);
        }

        GameDetails.Level = levels[0];
        dropdown.SelectOption(levels[0].levelName);
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
        GameDetails.Level = levels[index];
    }

    public List<LevelObject> LoadLevels() {
        return new List<LevelObject>(Resources.LoadAll<LevelObject>("Levels"));
    }

    public List<string> LoadMapsLEGACY() {
        List<string> filePaths = new List<string>(Directory.GetFiles(Application.streamingAssetsPath, "*.json",
                                         SearchOption.AllDirectories));

        return filePaths.Select((filepath) => {
            return filepath.Replace(Application.streamingAssetsPath + "\\", "").Replace(".json", "");
        }).ToList();
    }
}