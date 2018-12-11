using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardManager : MonoBehaviour {

    public const int DECK_SIZE = 30;

    public static CardManager singleton;

    [HideInInspector]
    public AbilityCardBase[] cards;

    private void Awake() {
        singleton = this;
        cards = Resources.LoadAll<AbilityCardBase>("Cards");
    }

    public Queue<AbilityCardBase> CreateDeck() {
        Queue<AbilityCardBase> newDeck = new Queue<AbilityCardBase>();

        for (int i=0; i < DECK_SIZE; ++i) {
            newDeck.Enqueue(cards[i%cards.Length]);
        }

        return newDeck;
    }


}
