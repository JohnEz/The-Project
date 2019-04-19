using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour {
    public static ResourceManager instance;

    [HideInInspector]
    public Dictionary<string, Ability> cards = new Dictionary<string, Ability>();

    [HideInInspector]
    public Dictionary<string, UnitObject> units = new Dictionary<string, UnitObject>();

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        LoadCards();
        LoadUnits();
    }

    private void LoadCards() {
        Ability[] loadedCards = Resources.LoadAll<Ability>("Cards");

        for (int i = 0; i < loadedCards.Length; i++) {
            Ability newCard = loadedCards[i];
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