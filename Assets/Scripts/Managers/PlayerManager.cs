using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class PlayerManager : NetworkBehaviour {
    // TODO this should be loaded in
    [SerializeField]
    List<AbilityCardBase> startingCards = new List<AbilityCardBase>();

    public List<PlayerData> players = new List<PlayerData>();

    public int humanCount = 0;
    //int cpuCount = 0;

    public void AddPlayer(short id) {
        humanCount++;

        PlayerData newPlayer = new PlayerData();
        newPlayer.id = players.Count;
        newPlayer.playerName = string.Format("Player %s", humanCount);
        newPlayer.ai = false;
        newPlayer.faction = 1;
        newPlayer.deck = CreateDeck(startingCards);

        //// TODO i need to clean this up and let it scale probably
        //if (humanCount == 1) {
        //    //newPlayer.myHand = player1Hand.GetComponent<Hand>();
        //    newPlayer.myDeck = player1Deck.GetComponent<Deck>();
        //    player1Deck.GetComponent<Deck>().SetPlayer(newPlayer);
        //} else {
        //    //newPlayer.myHand = player2Hand.GetComponent<Hand>();
        //    newPlayer.myDeck = player2Deck.GetComponent<Deck>();
        //    player2Deck.GetComponent<Deck>().SetPlayer(newPlayer);
        //}

        Debug.Log("Added player: " + id + " at index: " + players.Count);
        players.Add(newPlayer);
        
    }

    //public void AddAiPlayer(int faction, string name = null) {
    //    cpuCount++;
    //    Player newPlayer = new Player();
    //    newPlayer.id = players.Count;
    //    newPlayer.name = name != null ? name : string.Format("CPU %s", cpuCount);
    //    newPlayer.ai = true;
    //    newPlayer.faction = faction;
    //    newPlayer.deck = CreateDeck(startingCards);
    //    players.Add(newPlayer);
    //}


    private Queue<AbilityCardBase> CreateDeck(List<AbilityCardBase> _startingCards) {
        Queue<AbilityCardBase> newDeck = new Queue<AbilityCardBase>();

        startingCards.ForEach(card => {
            newDeck.Enqueue(card);
        });

        return newDeck;
    }

    public void StartNewTurn(int playerId) {
        StartNewTurn(GetPlayer(playerId));
    }

    public void StartNewTurn(PlayerData player) {
        //player.myDeck.DrawCard();
    }

    public int GetNumberOfPlayers() {
        return players.Count;
    }

    public PlayerData GetPlayer(int playerId) {
        return players[playerId];
    }

    //NETWORKING
}
