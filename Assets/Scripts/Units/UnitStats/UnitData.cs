using UnityEngine;
using System.Collections;

[System.Serializable]
public class UnitData {
    public string characterName;
    public string unitPrefab;
    public int level;
    public int xp;

    public UnitData() {
    }

    public UnitData(UnitObject unit) {
        characterName = unit.characterName;
        unitPrefab = unit.name.Replace("(Clone)", "");
        level = 0;
        xp = 0;
    }
}