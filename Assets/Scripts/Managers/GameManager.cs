using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    private const bool ADD_ALLY = true;

    private void Awake() {
        instance = this;
    }

    // Use this for initialization
    private void Start() {
        TileMap.instance.Initialise();

        UnitManager.instance.Initialise();
        CameraManager.instance.Initialise();

        //TEMP this should be loaded
        AddPlayers();
        AddObjectives();

        AudioManager.instance.PlayMusic("Battle", true);
    }

    public void StartGame() {
        TurnManager.instance.StartGame();
    }

    //TEMP
    private void AddPlayers() {
        Player humanPlayer = PlayerManager.instance.AddPlayer(1, "Jonesy");

        if (ADD_ALLY) {
            Player allyAI = PlayerManager.instance.AddAiPlayer(1);
        }

        if (GameDetails.Party.Count == 0 && Debug.isDebugBuild) {
            UnitManager.instance.SpawnUnit("Fighter", humanPlayer, 4, 12);
            UnitManager.instance.SpawnUnit("Cleric", humanPlayer, 5, 12);
            UnitManager.instance.SpawnUnit("Criminal", humanPlayer, 6, 12);
        } else {
            LoadPlayerCharacters(humanPlayer);
        }

        CameraManager.instance.JumpToLocation(humanPlayer.units[0].myNode);

        Player enemyAI = PlayerManager.instance.AddAiPlayer(2);

        //SpawnStartMapUnits(enemyAI);
        LoadMapUnits(enemyAI);

        TileMap.instance.ActivateRoom(humanPlayer.units[0].myNode.room);
    }

    private void LoadPlayerCharacters(Player humanPlayer) {
        List<SpawnLocation> playerSpawnLocation = TileMap.instance.spawnLocations.FindAll(sl => sl.name == "PlayerSpawn");

        int i = 0;
        GameDetails.Party.ForEach((character) => {
            if (i >= playerSpawnLocation.Count) {
                Debug.LogError("No spawn location for player unit!");
                return;
            }

            SpawnLocation spawnLocation = playerSpawnLocation[i];

            UnitManager.instance.SpawnUnit(character, humanPlayer, playerSpawnLocation[i].x, playerSpawnLocation[i].y);
            i++;
        });
    }

    //TEMP
    private void AddObjectives() {
        Objective objective = new Objective();
        objective.optional = false;
        objective.text = "Kill all enemies!";
        objective.type = ObjectiveType.ANNIHILATION;
        ObjectiveManager.instance.AddObjective(PlayerManager.instance.GetPlayer(0), objective);

        Objective objective2 = new Objective();
        objective2.optional = false;
        objective2.text = "Kill all enemies!";
        objective2.type = ObjectiveType.ANNIHILATION;
        ObjectiveManager.instance.AddObjective(PlayerManager.instance.GetPlayer(1), objective2);

        if (ADD_ALLY) {
            Objective objective3 = new Objective();
            objective3.optional = false;
            objective3.text = "Kill all enemies!";
            objective3.type = ObjectiveType.ANNIHILATION;
            ObjectiveManager.instance.AddObjective(PlayerManager.instance.GetPlayer(2), objective3);
        }
    }

    private void LoadMapUnits(Player enemyAI) {
        TileMap.instance.spawnLocations.ForEach(sl => {
            if (sl.name != "PlayerSpawn") {
                UnitManager.instance.SpawnUnit(sl.name, enemyAI, sl.x, sl.y);
            }
        });
    }

    private void SpawnStartMapUnits(Player enemyAI) {
        UnitManager.instance.SpawnUnit("Goblin Warrior", enemyAI, 7, 2);
        UnitManager.instance.SpawnUnit("Goblin Archer", enemyAI, 5, 1);
        UnitManager.instance.SpawnUnit("Goblin Warrior", enemyAI, 3, 1);

        UnitManager.instance.SpawnUnit("Goblin Warrior", enemyAI, 4, 9);
        //UnitManager.instance.SpawnUnit("Goblin Warrior", enemyAI, 5, 10);
        UnitManager.instance.SpawnUnit("Goblin Warrior", enemyAI, 6, 9);
    }

    // TEMP
    private void SpawnDungeon1Units(Player enemyAI) {
        UnitManager.instance.SpawnUnit("Goblin Warrior", enemyAI, 5, 8);
        UnitManager.instance.SpawnUnit("Goblin Warrior", enemyAI, 5, 9);
        UnitManager.instance.SpawnUnit("Goblin Warrior", enemyAI, 5, 10);

        UnitManager.instance.SpawnUnit("Goblin Archer", enemyAI, 14, 8);
        UnitManager.instance.SpawnUnit("Goblin Archer", enemyAI, 14, 10);

        UnitManager.instance.SpawnUnit("Goblin Warrior", enemyAI, 10, 2);
        UnitManager.instance.SpawnUnit("Goblin Archer", enemyAI, 11, 1);
        UnitManager.instance.SpawnUnit("Goblin Warrior", enemyAI, 12, 2);
    }
}