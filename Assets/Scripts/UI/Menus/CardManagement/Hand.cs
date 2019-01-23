using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {
    public static int HAND_LIMIT = 10;

    public GameObject cardPrefab;

    public PlayerUnit myUnit;
    public List<GameObject> myCards = new List<GameObject>();

    public void SetUnit(PlayerUnit _myUnit) {
        myUnit = _myUnit;
    }

    public void AddCardToHand(AbilityCardBase card) {
        if (myCards.Count < HAND_LIMIT) {
            GameObject cardObject = Instantiate(cardPrefab, transform);
            cardObject.GetComponent<CardDisplay>().ability = Instantiate(card);
            cardObject.GetComponent<CardDisplay>().myUnit = myUnit;
            cardObject.GetComponent<CardDisplay>().myHand = this;
            myCards.Add(cardObject);
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