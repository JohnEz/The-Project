using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager singleton;

    private const bool ADD_ALLY = false;

    private void Awake() {
        singleton = this;
    }

    // Use this for initialization
    private void Start() {
        TileMap.instance.Initialise();

        UnitManager.singleton.Initialise();
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
    private void AddPlayers() {
        Player humanPlayer = PlayerManager.singleton.AddPlayer(1, CardManager.singleton.LoadDeck(GameDetails.PlayerDeck), "Jonesy");

        if (ADD_ALLY) {
            Player allyAI = PlayerManager.singleton.AddAiPlayer(1);

            UnitManager.singleton.SpawnUnit("Criminal", allyAI, 2, 12);
            UnitManager.singleton.SpawnUnit("Farmer", allyAI, 3, 11);
            UnitManager.singleton.SpawnUnit("Scribe", allyAI, 4, 12);
        }

        SpawnLocation playerSpawnLocation = TileMap.instance.spawnLocations.Find(sl => sl.name == "PlayerSpawn");
        if (playerSpawnLocation != null) {
            humanPlayer.myCharacter = UnitManager.singleton.SpawnUnit(GameDetails.PlayerCharacter, PlayerManager.singleton.GetPlayer(0), playerSpawnLocation.x, playerSpawnLocation.y);
        } else {
            //humanPlayer.myCharacter = UnitManager.singleton.SpawnUnit(GameDetails.PlayerCharacter, PlayerManager.singleton.GetPlayer(0), 3, 12);
            humanPlayer.myCharacter = UnitManager.singleton.SpawnUnit(GameDetails.PlayerCharacter, PlayerManager.singleton.GetPlayer(0), 5, 6);
        }

        CameraManager.singleton.JumpToLocation(humanPlayer.myCharacter.myNode);

        Player enemyAI = PlayerManager.singleton.AddAiPlayer(2);

        // TileMap.instance.spawnLocations.ForEach(sl => {
        //     if (sl.name != "PlayerSpawn") {
        //         UnitManager.singleton.SpawnUnit(sl.name, enemyAI, sl.x, sl.y);
        //     }
        // });

        SpawnStartMapUnits(enemyAI);

        TileMap.instance.ActivateRoom(humanPlayer.myCharacter.myNode.room);
    }

    //TEMP
    private void AddObjectives() {
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

    private void SpawnStartMapUnits(Player enemyAI) {
        UnitManager.singleton.SpawnUnit("Goblin Archer", enemyAI, 7, 2);
        UnitManager.singleton.SpawnUnit("Goblin Archer", enemyAI, 5, 1);
        UnitManager.singleton.SpawnUnit("Goblin Archer", enemyAI, 3, 1);
    }

    // TEMP
    private void SpawnDungeon1Units(Player enemyAI) {
        UnitManager.singleton.SpawnUnit("Goblin Warrior", enemyAI, 5, 8);
        UnitManager.singleton.SpawnUnit("Goblin Warrior", enemyAI, 5, 9);
        UnitManager.singleton.SpawnUnit("Goblin Warrior", enemyAI, 5, 10);

        UnitManager.singleton.SpawnUnit("Goblin Archer", enemyAI, 14, 8);
        UnitManager.singleton.SpawnUnit("Goblin Archer", enemyAI, 14, 10);

        UnitManager.singleton.SpawnUnit("Goblin Warrior", enemyAI, 10, 2);
        UnitManager.singleton.SpawnUnit("Goblin Archer", enemyAI, 11, 1);
        UnitManager.singleton.SpawnUnit("Goblin Warrior", enemyAI, 12, 2);
    }
}