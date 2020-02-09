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

        if (GameDetails.Party.Count == 0 && Debug.isDebugBuild) {
            List<SpawnLocation> playerSpawnLocation = TileMap.instance.spawnLocations.FindAll(sl => sl.name == "PlayerSpawn");

            UnitManager.instance.SpawnUnit("Wanderer", humanPlayer, playerSpawnLocation[0].x, playerSpawnLocation[0].y);
            UnitManager.instance.SpawnUnit("Cleric", humanPlayer, playerSpawnLocation[1].x, playerSpawnLocation[1].y);
            UnitManager.instance.SpawnUnit("Criminal", humanPlayer, playerSpawnLocation[2].x, playerSpawnLocation[2].y);
            UnitManager.instance.SpawnUnit("Ranger", humanPlayer, playerSpawnLocation[3].x, playerSpawnLocation[3].y);
        } else {
            LoadPlayerCharacters(humanPlayer);
        }

        CameraManager.instance.JumpToLocation(humanPlayer.units[0].myTile);

        Player enemyAI = PlayerManager.instance.AddAiPlayer(2);
        Player allyAI = PlayerManager.instance.AddAiPlayer(1);

        LoadMapUnits(enemyAI, allyAI);
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

    private void AddObjectives() {
        GameDetails.Level.playerObjectives.ForEach(objective => {
            ObjectiveManager.instance.AddObjective(PlayerManager.instance.GetPlayer(0), objective);
        });

        // Temp
        if (GameDetails.Level.playerObjectives.Count == 0) {
            Objective objective = new Objective();
            objective.optional = false;
            objective.title = "ANNIHILATION";
            objective.text = "Kill all enemies!";
            objective.type = ObjectiveType.ANNIHILATION;
            ObjectiveManager.instance.AddObjective(PlayerManager.instance.GetPlayer(0), objective);
        }

        // Temp
        Objective objective2 = new Objective();
        objective2.optional = false;
        objective2.title = "ANNIHILATION";
        objective2.text = "Kill all enemies!";
        objective2.type = ObjectiveType.ANNIHILATION;
        ObjectiveManager.instance.AddObjective(PlayerManager.instance.GetPlayer(1), objective2);

        ObjectiveManager.instance.UpdateObjectives();
    }

    private void LoadMapUnits(Player enemyAI, Player allyAI) {
        TileMap.instance.spawnLocations.ForEach(sl => {
            if (sl.name != "PlayerSpawn") {
                UnitManager.instance.SpawnUnit(sl.name, sl.isAllied ? allyAI : enemyAI, sl.x, sl.y);
            }
        });
    }
}