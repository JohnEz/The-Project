using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour {
    public GameObject cardPrefab;
    public Transform hand;

    Player myPlayer;

    // Use this for initialization
    void Start() {

    }

    public void SetPlayer(Player _myPlayer) {
        myPlayer = _myPlayer;
    }

    public void DrawCard(int count = 1) {
        for(int i=0; i < count; ++i) {
            DrawCard();
        }
    }

    private void DrawCard() {
        if (myPlayer.deck.Count > 0) {
            AbilityCardBase drawnCardAbility = myPlayer.deck.Dequeue();
            GameObject cardObject = Instantiate(cardPrefab, hand);
            cardObject.GetComponent<CardDisplay>().ability = drawnCardAbility;
            cardObject.GetComponent<CardDisplay>().myPlayer = myPlayer;
        } else {
            Debug.Log("No cards left to draw");
        }
    }

    public void Shuffle() {

    }
}
