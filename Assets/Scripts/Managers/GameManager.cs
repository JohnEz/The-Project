using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public static GameManager singleton;

    TileMap map;

    private const bool ADD_ALLY = false;

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

        AudioManager.singleton.PlayMusic("Battle", true);
    }

    public void StartGame() {
        TurnManager.singleton.StartGame();
    }

    //TEMP
    void AddPlayers() {
        Player humanPlayer = PlayerManager.singleton.AddPlayer(1, CardManager.singleton.LoadDeck(GameDetails.PlayerDeck), "Jonesy");

        if (ADD_ALLY) {
            Player allyAI = PlayerManager.singleton.AddAiPlayer(1);

            UnitManager.singleton.SpawnUnit("Bandit", allyAI, 1, 8);
            UnitManager.singleton.SpawnUnit("Bandit", allyAI, 1, 10);
        }

        humanPlayer.myCharacter = UnitManager.singleton.SpawnUnit(GameDetails.PlayerCharacter, PlayerManager.singleton.GetPlayer(0), 1, 9);
        CameraManager.singleton.JumpToLocation(humanPlayer.myCharacter.myNode);

        Player enemyAI = PlayerManager.singleton.AddAiPlayer(2);

        UnitManager.singleton.SpawnUnit("Bandit", enemyAI, 5, 8);
        UnitManager.singleton.SpawnUnit("Bandit", enemyAI, 5, 9);
        UnitManager.singleton.SpawnUnit("Bandit", enemyAI, 5, 10);

        UnitManager.singleton.SpawnUnit("Bandit Archer", enemyAI, 14, 8);
        UnitManager.singleton.SpawnUnit("Bandit Archer", enemyAI, 14, 10);

        UnitManager.singleton.SpawnUnit("Bandit", enemyAI, 10, 2);
        UnitManager.singleton.SpawnUnit("Bandit Archer", enemyAI, 11, 1);
        UnitManager.singleton.SpawnUnit("Bandit", enemyAI, 12, 2);
        
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

        if (ADD_ALLY) {
            Objective objective3 = new Objective();
            objective3.optional = false;
            objective3.text = "Kill all enemies!";
            objective3.type = ObjectiveType.ANNIHILATION;
            ObjectiveManager.singleton.AddObjective(PlayerManager.singleton.GetPlayer(2), objective3);
        }
    }
}
