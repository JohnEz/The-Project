using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    public static GameManager singleton;

    private const bool ADD_ALLY = true;

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
        Player humanPlayer = PlayerManager.singleton.AddPlayer(1, "Jonesy");

        if (ADD_ALLY) {
            Player allyAI = PlayerManager.singleton.AddAiPlayer(1);

            //UnitManager.singleton.SpawnUnit("Criminal", allyAI, 4, 12);

            if (GameDetails.PlayerCharacter == "Fighter") {
                //UnitManager.singleton.SpawnUnit("Scribe", allyAI, 6, 12);
            } else {
                //UnitManager.singleton.SpawnUnit("Farmer", allyAI, 6, 12);
            }
        }

        List<AbilityCardBase> fighterdeck = CardManager.singleton.LoadDeck(BasicDecks.starterFighter);
        List<AbilityCardBase> barddeck = CardManager.singleton.LoadDeck(BasicDecks.starterBard);
        List<AbilityCardBase> clericdeck = CardManager.singleton.LoadDeck(BasicDecks.starterCleric);
        List<AbilityCardBase> elementalistdeck = CardManager.singleton.LoadDeck(BasicDecks.starterElementalist);
        //SpawnLocation playerSpawnLocation = TileMap.instance.spawnLocations.Find(sl => sl.name == "PlayerSpawn");
        //int playerSpawnX = playerSpawnLocation != null ? playerSpawnLocation.x : 5;
        //int playerSpawnY = playerSpawnLocation != null ? playerSpawnLocation.y : 6;
        //UnitManager.singleton.SpawnPlayerUnit(GameDetails.PlayerCharacter, humanPlayer, playerSpawnX, playerSpawnY, fighterdeck);
        UnitManager.singleton.SpawnPlayerUnit("Fighter", humanPlayer, 4, 12, fighterdeck);
        UnitManager.singleton.SpawnPlayerUnit("Cleric", humanPlayer, 5, 12, clericdeck);
        UnitManager.singleton.SpawnPlayerUnit("Mage", humanPlayer, 6, 12, elementalistdeck);

        CameraManager.singleton.JumpToLocation(humanPlayer.units[0].unit.myNode);

        Player enemyAI = PlayerManager.singleton.AddAiPlayer(2);

        SpawnStartMapUnits(enemyAI);

        TileMap.instance.ActivateRoom(humanPlayer.units[0].unit.myNode.room);
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

    private void LoadMapUnits(Player enemyAI) {
        TileMap.instance.spawnLocations.ForEach(sl => {
            if (sl.name != "PlayerSpawn") {
                UnitManager.singleton.SpawnUnit(sl.name, enemyAI, sl.x, sl.y);
            }
        });
    }

    private void SpawnStartMapUnits(Player enemyAI) {
        UnitManager.singleton.SpawnUnit("Goblin Warrior", enemyAI, 7, 2);
        UnitManager.singleton.SpawnUnit("Goblin Archer", enemyAI, 5, 1);
        UnitManager.singleton.SpawnUnit("Goblin Warrior", enemyAI, 3, 1);

        UnitManager.singleton.SpawnUnit("Goblin Warrior", enemyAI, 4, 9);
        //UnitManager.singleton.SpawnUnit("Goblin Warrior", enemyAI, 5, 10);
        UnitManager.singleton.SpawnUnit("Goblin Warrior", enemyAI, 6, 9);
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