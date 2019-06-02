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

        Player allyAI = PlayerManager.instance.AddAiPlayer(1);

        if (GameDetails.Party.Count == 0 && Debug.isDebugBuild) {
            List<SpawnLocation> playerSpawnLocation = TileMap.instance.spawnLocations.FindAll(sl => sl.name == "PlayerSpawn");

            UnitManager.instance.SpawnUnit("Fighter", humanPlayer, playerSpawnLocation[0].x, playerSpawnLocation[0].y);
            UnitManager.instance.SpawnUnit("Cleric", humanPlayer, playerSpawnLocation[1].x, playerSpawnLocation[1].y);
            UnitManager.instance.SpawnUnit("Criminal", humanPlayer, playerSpawnLocation[2].x, playerSpawnLocation[2].y);
        } else {
            LoadPlayerCharacters(humanPlayer);
        }

        CameraManager.instance.JumpToLocation(humanPlayer.units[0].myNode);

        Player enemyAI = PlayerManager.instance.AddAiPlayer(2);

        LoadMapUnits(enemyAI, allyAI);

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

    private void LoadMapUnits(Player enemyAI, Player allyAI) {
        TileMap.instance.spawnLocations.ForEach(sl => {
            if (sl.name != "PlayerSpawn") {
                UnitManager.instance.SpawnUnit(sl.name, sl.isAllied ? allyAI : enemyAI, sl.x, sl.y);
            }
        });
    }
}