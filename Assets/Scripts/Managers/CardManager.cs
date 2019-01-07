using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour {
    public const int DECK_SIZE = 30;

    public static CardManager singleton;

    public Draggable currentlyDraggedCard; // TODO this should be moved

    private void Awake() {
        singleton = this;
    }

    public List<AbilityCardBase> LoadDeck(List<string> deckList) {
        List<AbilityCardBase> loadedDeck = new List<AbilityCardBase>();

        deckList.ForEach(cardName => {
            AbilityCardBase cardToAdd = ResourceManager.singleton.cards[cardName];

            if (cardToAdd != null) {
                loadedDeck.Add(cardToAdd);
            }
        });

        return loadedDeck;
    }
}