using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardManager : MonoBehaviour {

    public const int DECK_SIZE = 30;

    public static CardManager singleton;

    [HideInInspector]
    public Dictionary<string, AbilityCardBase> cards = new Dictionary<string, AbilityCardBase>();

    private void Awake() {
        singleton = this;
        AbilityCardBase[] loadedCards = Resources.LoadAll<AbilityCardBase>("Cards");

        for(int i=0; i < loadedCards.Length; i++) {
            AbilityCardBase newCard = loadedCards[i];
            cards.Add(newCard.name, newCard);
        }
    }

    public List<AbilityCardBase> CreateDeck() {
        List<AbilityCardBase> newDeck = new List<AbilityCardBase>();

        //for (int i=0; i < DECK_SIZE; ++i) {
        //    newDeck.Enqueue(cards[i%cards.Length]);
        //}

        return newDeck;
    }

    public List<AbilityCardBase> LoadDeck(List<string> deckList) {
        List<AbilityCardBase> loadedDeck = new List<AbilityCardBase>();

        deckList.ForEach(cardName => {
            AbilityCardBase cardToAdd = cards[cardName];

            if (cardToAdd != null) {
                loadedDeck.Add(cardToAdd);
            }
        });

        return loadedDeck;
    }


}
