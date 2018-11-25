using UnityEngine;
using UnityEngine.Networking;

public struct CardId {
    public int id;
    public string name;
}

public class CardMessage : MessageBase {
    public NetworkInstanceId playerId;
    public CardId cardId;
    public static short CardMsgId = 1000;
}

public class CardManager : NetworkBehaviour {

    public static CardManager singleton;

    //TODO move this to its own file or manager with IDs
    public AbilityCardBase[] cards;

    // TODO scalable?
    public Deck p1Deck;
    public Deck p2Deck;

    private void Awake() {
        singleton = this;
        cards = Resources.LoadAll<AbilityCardBase>("Cards");
    }

    public CardId GetRandomCard(int id) {
        if (id == 0) {
            return p1Deck.GetTopCard();
        } else {
            return p2Deck.GetTopCard();
        }
        
    }

    [Server]
    public void ServerSetupDeck(int playerId) {
        Debug.Log("Setup Deck for player " + playerId);
        if (playerId == 0) {
            p1Deck.SetupDeck();
        } else {
            p2Deck.SetupDeck();
        }
    }
}
