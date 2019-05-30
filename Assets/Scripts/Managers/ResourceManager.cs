using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour {
    public static ResourceManager instance;

    [HideInInspector]
    public Dictionary<string, UnitObject> units = new Dictionary<string, UnitObject>();

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        LoadUnits();
    }

    private void LoadUnits() {
        UnitObject[] loadedUnits = Resources.LoadAll<UnitObject>("Units");

        for (int i = 0; i < loadedUnits.Length; i++) {
            UnitObject newUnit = loadedUnits[i];
            units.Add(newUnit.className, newUnit);
        }
    }
}