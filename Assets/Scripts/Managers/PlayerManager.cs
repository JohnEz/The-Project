using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player {
    public int id;
    public string name;
    public bool ai;
    public int faction;

    public Queue<AbilityCardBase> deck = new Queue<AbilityCardBase>();
    List<AbilityCardBase> hand = new List<AbilityCardBase>();
    public Stack<AbilityCardBase> discard = new Stack<AbilityCardBase>();

    // Physical world objects
    public UnitController myCharacter;
    //public Hand myHand;
    public Deck myDeck;
}

public class PlayerManager : MonoBehaviour {
    public static PlayerManager singleton;

    List<Player> players = new List<Player>();

    public GameObject player1Hand;
    public GameObject player1Deck;

    int humanCount = 0;
    int cpuCount = 0;

    private void Awake() {
        singleton = this;
    }

    public void AddPlayer(int faction, Queue<AbilityCardBase> deck, string name = null) {
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
        }

        players.Add(newPlayer);
    }

    public void AddAiPlayer(int faction, string name = null) {
        cpuCount++;
        Player newPlayer = new Player();
        newPlayer.id = players.Count;
        newPlayer.name = name != null ? name : string.Format("CPU %s", cpuCount);
        newPlayer.ai = true;
        newPlayer.faction = faction;
        players.Add(newPlayer);
    }

    public void StartNewTurn(int playerId) {
        StartNewTurn(GetPlayer(playerId));
    }

    public void StartNewTurn(Player player) {
        player.myDeck.DrawCard(5);
    }

    public int GetNumberOfPlayers() {
        return players.Count;
    }

    public Player GetPlayer(int playerId) {
        return players[playerId];
    }
}
