using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public static class SaveSystem {
    public static string saveFile = "/saveData.osm";

    public const string ROSTER = "roster";
    public const string ENCOUNTERS = "encounters";

    public static void Save() {
        SaveRoster();
        SaveEncounteredEnemies();
    }

    public static void Load() {
        LoadRoster();
        LoadEncounteredEnemies();
    }

    public static void SaveRoster() {
        List<UnitData> rosterData = new List<UnitData>();

        PlayerSchool.Roster.ForEach(unit => {
            rosterData.Add(new UnitData(unit));
        });

        ES2.Save(rosterData.ToArray(), ROSTER);
    }

    public static void LoadRoster() {
        UnitData[] rosterData = ES2.LoadArray<UnitData>(ROSTER);
        PlayerSchool.Roster.Clear();

        foreach (UnitData data in rosterData) {
            if (!ResourceManager.instance.units.ContainsKey(data.unitPrefab)) {
                Debug.LogError("Could not find character prefab " + data.unitPrefab);
                return;
            }

            UnitObject loadedUnit = Object.Instantiate(ResourceManager.instance.units[data.unitPrefab]);
            loadedUnit.characterName = data.characterName;

            PlayerSchool.Roster.Add(loadedUnit);
        }
    }

    public static void SaveEncounteredEnemies() {
        ES2.Save(SavedVariables.encounteredEnemies.ToArray(), ENCOUNTERS);
    }

    public static void LoadEncounteredEnemies() {
        foreach (string enemy in ES2.LoadArray<string>(ENCOUNTERS)) {
            SavedVariables.EncounteredEnemy(enemy);
        }
    }
}