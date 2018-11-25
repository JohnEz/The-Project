using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Deck : NetworkBehaviour {
    // TODO convert ints to cardIds
    public List<CardId> cards = new List<CardId>();
    public List<CardId> usedCards = new List<CardId>();

    public const int DECK_SIZE = 30;

    public void SetupDeck() {
        //TODO give actual decks
        for (int i = 0; i < DECK_SIZE; ++i) {
            CardId newCard = new CardId();
            newCard.id = UnityEngine.Random.Range(0, CardManager.singleton.cards.Length);
            newCard.name = CardManager.singleton.cards[newCard.id].name;
            cards.Add(newCard);
        }

        Shuffle();
    }

    public void Shuffle() {
        List<CardId> tmp = new List<CardId>();

        int max = cards.Count;
        while (max > 0) {
            int offset = UnityEngine.Random.Range(0, max);
            tmp.Add(cards[offset]);
            cards.RemoveAt(offset);
            max -= 1;
        }
        cards = tmp;
    }

    public CardId GetTopCard() {
        // TODO this should be count -1 but because im using index as id i cant
        int top = cards.Count - 1;

        CardId drawnCard = cards[top];
        cards.RemoveAt(top);
        usedCards.Add(drawnCard);

        //Debug.Log("Got top:" + top + " " + cardId);
        return drawnCard;
    }

    // Use this for initialization
    void Start() {

    }

}
