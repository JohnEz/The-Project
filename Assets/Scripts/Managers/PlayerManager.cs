using System.Collections.Generic;
using UnityEngine;

public class Player {
    public int id;
    public string name;
    public bool ai;
    public int faction;

    public void StartTurn() {
    }

    // Physical world objects

    public List<UnitController> units = new List<UnitController>();
}

public class PlayerManager : MonoBehaviour {
    public static PlayerManager instance;

    private List<Player> players = new List<Player>();

    private int humanCount = 0;
    private int cpuCount = 0;

    //TODO i dont like this, can we store it in a player as local player or current player?
    public Player mainPlayer;

    private void Awake() {
        instance = this;
    }

    public Player AddPlayer(int faction, string name = null) {
        humanCount++;
        Player newPlayer = new Player();
        newPlayer.id = players.Count;
        newPlayer.name = name != null ? name : string.Format("Player %s", humanCount);
        newPlayer.ai = false;
        newPlayer.faction = faction;

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
    }

    public void StartNewTurn(int playerId) {
        StartNewTurn(GetPlayer(playerId));
    }

    public void StartNewTurn(Player player) {
        if (!player.ai) {
            player.StartTurn();
        }
    }

    public void EndTurn(Player player) {
    }

    public int GetNumberOfPlayers() {
        return players.Count;
    }

    public Player GetPlayer(int playerId) {
        return players[playerId];
    }

    public bool IsMainPlayer(int playerId) {
        return players[playerId] == mainPlayer;
    }
}