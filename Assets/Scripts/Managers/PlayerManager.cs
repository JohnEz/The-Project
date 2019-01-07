using System.Collections.Generic;
using UnityEngine;

public class Player {
    public int id;
    public string name;
    public bool ai;
    public int faction;

    public List<AbilityCardBase> deck = new List<AbilityCardBase>();
    private List<AbilityCardBase> hand = new List<AbilityCardBase>();
    public Stack<AbilityCardBase> discard = new Stack<AbilityCardBase>();

    // Physical world objects
    public UnitController myCharacter;

    //public Hand myHand;
    public Deck myDeck;
}

public class PlayerManager : MonoBehaviour {
    public static PlayerManager singleton;

    private List<Player> players = new List<Player>();

    public GameObject player1Hand;
    public GameObject player1Deck;

    private int humanCount = 0;
    private int cpuCount = 0;

    //TODO i dont like this, can we store it in a player as local player or current player?
    public Player mainPlayer;

    private void Awake() {
        singleton = this;
    }

    public Player AddPlayer(int faction, List<AbilityCardBase> deck, string name = null) {
        humanCount++;
        Player newPlayer = new Player();
        newPlayer.id = players.Count;
        newPlayer.name = name != null ? name : string.Format("Player %s", humanCount);
        newPlayer.ai = false;
        newPlayer.faction = faction;
        newPlayer.deck = deck;

        // TODO i need to clean this up and let it scale probably
        if (humanCount == 1) {
            //newPlayer.myHand = player1Hand.GetComponent<Hand>();
            newPlayer.myDeck = player1Deck.GetComponent<Deck>();
            player1Deck.GetComponent<Deck>().SetPlayer(newPlayer);
            player1Hand.GetComponent<Hand>().SetPlayer(newPlayer);
        }

        mainPlayer = newPlayer;

        players.Add(newPlayer);

        return newPlayer;
    }

    public Player AddAiPlayer(int faction, string name = null) {
        cpuCount++;
        Player newPlayer = new Player();
        newPlayer.id = players.Count;
        newPlayer.name = name != null ? name : string.Format("CPU %s", cpuCount);
        newPlayer.ai = true;
        newPlayer.faction = faction;
        players.Add(newPlayer);
        return newPlayer;
    }

    public void StartGame() {
        // Draw starting Hand
        mainPlayer.myDeck.Shuffle();
        mainPlayer.myDeck.DrawCard(4);
    }

    public void StartNewTurn(int playerId) {
        StartNewTurn(GetPlayer(playerId));
    }

    public void StartNewTurn(Player player) {
        if (!player.ai) {
            player.myDeck.DrawCard(2);
        }
    }

    public int GetNumberOfPlayers() {
        return players.Count;
    }

    public Player GetPlayer(int playerId) {
        return players[playerId];
    }
}