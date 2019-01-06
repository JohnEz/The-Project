using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour {

    public static ResourceManager singleton;

    [HideInInspector]
    public Dictionary<string, AbilityCardBase> cards = new Dictionary<string, AbilityCardBase>();

    [HideInInspector]
    public Dictionary<string, GameObject> units = new Dictionary<string, GameObject>();

    private void Awake() {
        if (singleton != null) {
            Destroy(gameObject);
        } else {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }

        LoadCards();
        LoadUnits();
    }

    private void LoadCards() {
        AbilityCardBase[] loadedCards = Resources.LoadAll<AbilityCardBase>("Cards");

        for (int i = 0; i < loadedCards.Length; i++) {
            AbilityCardBase newCard = loadedCards[i];
            cards.Add(newCard.name, newCard);
        }
    }

    private void LoadUnits() {
        GameObject[] loadedUnits = Resources.LoadAll<GameObject>("Units");

        for (int i = 0; i < loadedUnits.Length; i++) {
            GameObject newUnit = loadedUnits[i];
            units.Add(newUnit.GetComponent<UnitController>().baseStats.className, newUnit);
        }
    }
}
