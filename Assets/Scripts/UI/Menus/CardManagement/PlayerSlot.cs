using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSlot : NetworkBehaviour {

    public PlayerConnectionObject myPlayer;

    public GameObject cardPrefab;

    public Deck myDeck;

    public Transform hand;

    public CardSlot[] cardSlots;

    private void Awake() {
        cardSlots = GetComponentsInChildren<CardSlot>();

        for(int i=0; i<cardSlots.Length; ++i) {
            cardSlots[i].slotIndex = i;
            cardSlots[i].myPlayer = myPlayer;
        }

        ClearCards();
    }

    public void AddCard(CardId cardId) {
        CardSlot cardSlot = GetFirstAvailableSlot();

        cardSlot.ShowCard(cardId);
    }

    private CardSlot GetFirstAvailableSlot() {
        foreach (CardSlot card in cardSlots) {
            if (!card.isActive) {
                return card;
            }
        }
        Debug.LogError("There were no available slots!?");
        return null;
    }

    public void PlayCard(CardId cardId, int slotIndex) {
        //validation
        if (slotIndex >= cardSlots.Length) {
            Debug.LogError("That slot doesn't exist");
            return;
        }
        if (!cardSlots[slotIndex].isActive) {
            Debug.LogError("This card was not active?!");
            return;
        }
        if (cardSlots[slotIndex].cardId.id != cardId.id) {
            Debug.LogError("That card was not in that slot?!");
        }
        cardSlots[slotIndex].ClearSlot();
    }

    public void ClearCards() {
        foreach (CardSlot card in cardSlots) {
            card.ClearSlot();
        }
    }

}
