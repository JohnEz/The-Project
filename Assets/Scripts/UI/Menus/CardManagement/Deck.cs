using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Deck : MonoBehaviour {
    public Hand hand;

    private PlayerUnit myUnit;

    // Use this for initialization
    private void Start() {
    }

    public void SetUnit(PlayerUnit _myUnit) {
        myUnit = _myUnit;
    }

    public void DrawCard(int count = 1) {
        for (int i = 0; i < count; ++i) {
            DrawCard();
        }
    }

    private void DrawCard() {
        if (myUnit.deckList.Count > 0) {
            AbilityCardBase drawnCardAbility = GetTopCard();
            hand.AddCardToHand(drawnCardAbility);
        }
    }

    // note this removes it from the deck, maybe we need a peek?
    private AbilityCardBase GetTopCard() {
        AbilityCardBase topCard = myUnit.deckList.Last();
        myUnit.deckList.RemoveAt(myUnit.deckList.Count - 1);
        return topCard;
    }

    public void Shuffle() {
        List<AbilityCardBase> tmp = new List<AbilityCardBase>();

        int max = myUnit.deckList.Count;
        while (max > 0) {
            int offset = UnityEngine.Random.Range(0, max);
            tmp.Add(myUnit.deckList[offset]);
            myUnit.deckList.RemoveAt(offset);
            max -= 1;
        }
        myUnit.deckList = tmp;
    }
}