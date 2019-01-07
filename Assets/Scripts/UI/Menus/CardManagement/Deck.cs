using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Deck : MonoBehaviour {
    public Hand hand;

    private Player myPlayer;

    // Use this for initialization
    private void Start() {
    }

    public void SetPlayer(Player _myPlayer) {
        myPlayer = _myPlayer;
    }

    public void DrawCard(int count = 1) {
        for (int i = 0; i < count; ++i) {
            DrawCard();
        }
    }

    private void DrawCard() {
        if (myPlayer.deck.Count > 0) {
            AbilityCardBase drawnCardAbility = GetTopCard();
            hand.AddCardToHand(drawnCardAbility);
        }
    }

    // note this removes it from the deck, maybe we need a peek?
    private AbilityCardBase GetTopCard() {
        AbilityCardBase topCard = myPlayer.deck.Last();
        myPlayer.deck.RemoveAt(myPlayer.deck.Count - 1);
        return topCard;
    }

    public void Shuffle() {
        List<AbilityCardBase> tmp = new List<AbilityCardBase>();

        int max = myPlayer.deck.Count;
        while (max > 0) {
            int offset = UnityEngine.Random.Range(0, max);
            tmp.Add(myPlayer.deck[offset]);
            myPlayer.deck.RemoveAt(offset);
            max -= 1;
        }
        myPlayer.deck = tmp;
    }
}