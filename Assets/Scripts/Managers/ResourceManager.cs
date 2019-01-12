using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour {
    public static ResourceManager singleton;

    [HideInInspector]
    public Dictionary<string, AbilityCardBase> cards = new Dictionary<string, AbilityCardBase>();

    [HideInInspector]
    public Dictionary<string, UnitObject> units = new Dictionary<string, UnitObject>();

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
        UnitObject[] loadedUnits = Resources.LoadAll<UnitObject>("Units");

        for (int i = 0; i < loadedUnits.Length; i++) {
            UnitObject newUnit = loadedUnits[i];
            units.Add(newUnit.className, newUnit);
        }
    }
}