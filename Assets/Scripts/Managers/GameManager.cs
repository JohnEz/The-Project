using UnityEngine;
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

        Invoke("StartGame", 2.0f);
    }

    void StartGame() {
        TurnManager.singleton.StartGame();
    }

    //TEMP
    void AddPlayers() {
        Player humanPlayer = PlayerManager.singleton.AddPlayer(1, CardManager.singleton.LoadDeck(BasicDecks.elementalistDeck), "Jonesy");

        if (MatchDetails.VersusAi) {
            Player cpuPlayer = PlayerManager.singleton.AddAiPlayer(2);
            Player cpuPlayer2 = PlayerManager.singleton.AddAiPlayer(1);
        } else {
            Player humanPlayer2 = PlayerManager.singleton.AddPlayer(2, CardManager.singleton.CreateDeck(), "Jimmy");
        }

        humanPlayer.myCharacter = UnitManager.singleton.SpawnUnit(5, PlayerManager.singleton.GetPlayer(0), 10, 10);

        UnitManager.singleton.SpawnUnit(2, PlayerManager.singleton.GetPlayer(2), 12, 9);
        UnitManager.singleton.SpawnUnit(2, PlayerManager.singleton.GetPlayer(2), 12, 11);

        UnitManager.singleton.SpawnUnit(2, PlayerManager.singleton.GetPlayer(1), 16, 9);
        UnitManager.singleton.SpawnUnit(6, PlayerManager.singleton.GetPlayer(1), 17, 10);
        UnitManager.singleton.SpawnUnit(2, PlayerManager.singleton.GetPlayer(1), 16, 11);
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
