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
        Debug.Log(cards);
    }

    public Queue<AbilityCardBase> CreateDeck() {
        Queue<AbilityCardBase> newDeck = new Queue<AbilityCardBase>();

        for (int i=0; i < DECK_SIZE; ++i) {
            Debug.Log("i" + i);
            Debug.Log("cards.Length" + i % cards.Length);
            Debug.Log("i%cards.Length" + i % cards.Length);
            newDeck.Enqueue(cards[i%cards.Length]);
        }

        return newDeck;
    }


}
