using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {
    public static int HAND_LIMIT = 6;

    public GameObject cardSlotPrefab;

    public PlayerUnit myUnit;
    public List<GameObject> myCards = new List<GameObject>();

    public void Awake() {
    }

    public void SetUnit(PlayerUnit _myUnit) {
        myUnit = _myUnit;
    }

    public void AddCardToHand(AbilityCardBase card) {
        if (myCards.Count < HAND_LIMIT) {
            GameObject cardSlot = Instantiate(cardSlotPrefab, transform);
            cardSlot.GetComponent<CardSlot>().card = Instantiate(card);
            cardSlot.GetComponent<CardSlot>().myUnit = myUnit;
            cardSlot.GetComponent<CardSlot>().myHand = this;
            myCards.Add(cardSlot);
        } else {
            // TODO create card and show burn animation
            GUIController.singleton.ShowErrorMessage("Burnt card " + card.name);
        }
    }

    public void CardDestroyed(GameObject destroyedCard) {
        if (myCards.Contains(destroyedCard)) {
            myCards.Remove(destroyedCard);
        }
    }
}