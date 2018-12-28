﻿using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public static GameManager singleton;

    TileMap map;

    private void Awake() {
        singleton = this;
    }

    // Use this for initialization
    void Start() {
        // TODO try to abstract this out
        map = GetComponentInChildren<TileMap>();
        map.Initialise();

        UnitManager.singleton.Initialise(map);
        AIManager.singleton.Initialise(map);
        CameraManager.singleton.Initialise();

        //TEMP this should be loaded
        AddPlayers();
        AddObjectives();
    }

    public void StartGame() {
        TurnManager.singleton.StartGame();
    }

    //TEMP
    void AddPlayers() {
        Player humanPlayer = PlayerManager.singleton.AddPlayer(1, CardManager.singleton.LoadDeck(BasicDecks.fireElementalist), "Jonesy");

        if (MatchDetails.VersusAi) {
            Player cpuPlayer = PlayerManager.singleton.AddAiPlayer(1);
            Player cpuPlayer2 = PlayerManager.singleton.AddAiPlayer(2);
        } else {
            Player humanPlayer2 = PlayerManager.singleton.AddPlayer(2, CardManager.singleton.CreateDeck(), "Jimmy");
        }

        humanPlayer.myCharacter = UnitManager.singleton.SpawnUnit(5, PlayerManager.singleton.GetPlayer(0), 12, 10);
        CameraManager.singleton.JumpToLocation(humanPlayer.myCharacter.myNode);

        UnitManager.singleton.SpawnUnit(2, PlayerManager.singleton.GetPlayer(1), 12, 9);
        UnitManager.singleton.SpawnUnit(2, PlayerManager.singleton.GetPlayer(1), 12, 11);

        UnitManager.singleton.SpawnUnit(2, PlayerManager.singleton.GetPlayer(2), 16, 9);
        UnitManager.singleton.SpawnUnit(6, PlayerManager.singleton.GetPlayer(2), 17, 10);
        UnitManager.singleton.SpawnUnit(2, PlayerManager.singleton.GetPlayer(2), 16, 11);
    }

    //TEMP
    void AddObjectives() {
        Objective objective = new Objective();
        objective.optional = false;
        objective.text = "Kill all enemies!";
        objective.type = ObjectiveType.ANNIHILATION;
        ObjectiveManager.singleton.AddObjective(PlayerManager.singleton.GetPlayer(0), objective);

        Objective objective2 = new Objective();
        objective2.optional = false;
        objective2.text = "Kill all enemies!";
        objective2.type = ObjectiveType.ANNIHILATION;
        ObjectiveManager.singleton.AddObjective(PlayerManager.singleton.GetPlayer(1), objective2);

        Objective objective3 = new Objective();
        objective2.optional = false;
        objective2.text = "Kill all enemies!";
        objective2.type = ObjectiveType.ANNIHILATION;
        ObjectiveManager.singleton.AddObjective(PlayerManager.singleton.GetPlayer(2), objective3);
    }
}
