using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public enum TurnPhase {
	TURN_STARTING,
	WAITING_FOR_INPUT,
	UNIT_MOVING,
	UNIT_ATTACKING,
	TURN_ENDING,
    WAITING_FOR_PLAYERS
}

public class TurnManager : NetworkBehaviour {

    public static TurnManager singleton;

    UserInterfaceManager uIManager;
	GUIController gUIController;
	UnitManager unitManager;
	AIManager aiManager;
	CameraManager cameraManager;
	TileMap map;
	ObjectiveManager objectiveManager;
    PlayerManager playerManager;

    public PlayerConnectionObject currentTurnPlayer;

    [SyncVar]
    TurnPhase currentPhase = TurnPhase.WAITING_FOR_PLAYERS;

    [SyncVar]
	int playersTurn = -1;

    [SyncVar]
    public bool gameHasStarted = false;

	bool checkedIfTurnShouldEnd = true;

    private void Awake() {
        singleton = this;
    }

    // Use this for initialization
    public void Initialise () {
		//uIManager = GetComponentInChildren<UserInterfaceManager> ();
		//gUIController = GetComponentInChildren<GUIController> ();
		//map = GetComponentInChildren<TileMap> ();
		//map.Initialise ();
		//unitManager = GetComponent<UnitManager> ();
		//unitManager.Initialise (map);
		//aiManager = GetComponent<AIManager> ();
		//aiManager.Initialise (map);
		//cameraManager = GetComponent<CameraManager> ();
		//cameraManager.Initialise ();
  //      playerManager = GetComponent<PlayerManager>();
  //      AddPlayers();
  //      objectiveManager = GetComponent<ObjectiveManager> ();
		//objectiveManager.Initialise ();
		//AddObjectives ();
		//StartNewTurn ();
	}
	
	// Update is called once per frame
	void Update () {
		//check to see if the turn should end
		//if (!checkedIfTurnShouldEnd) {
		//	if (!isAiTurn() && currentPhase == TurnPhase.WAITING_FOR_INPUT && unitManager.PlayerOutOfActions (playersTurn)) {
		//		EndTurn ();
		//	} else {
  //              checkedIfTurnShouldEnd = true;
		//	}
		//}
        
	}

	//TEMP
	void AddPlayers() {
        //playerManager.AddPlayer(1, "Jonesy");

        //if (MatchDetails.VersusAi) {
        //    playerManager.AddAiPlayer(2);
        //} else {
        //    playerManager.AddPlayer(2, "Jimmy");
        //}

        //unitManager.SpawnUnit(7, playerManager.GetPlayer(0), 16, 10);
        //unitManager.SpawnUnit(2, playerManager.GetPlayer(1), 17, 10);
    }

	//TEMP
	void AddObjectives() {
		//Objective objective = new Objective ();
		//objective.optional = false;
		//objective.text = "Kill all enemies!";
		//objective.type = ObjectiveType.ANNIHILATION;
		//objectiveManager.AddObjective (playerManager.GetPlayer(0), objective);

		//Objective objective2 = new Objective ();
		//objective2.optional = false;
		//objective2.text = "Kill all enemies!";
		//objective2.type = ObjectiveType.ANNIHILATION;
		//objectiveManager.AddObjective (playerManager.GetPlayer(1), objective2);
	}

	public void StartNewTurn() {
        //if (playerManager.GetNumberOfPlayers() > 0) {
        //    ChangeState(TurnPhase.TURN_STARTING);
        //    playersTurn++;
        //    playersTurn = playersTurn % playerManager.GetNumberOfPlayers();
        //    PlayerData currentPlayersTurn = GetCurrentPlayer();
        //    unitManager.StartTurn(currentPlayersTurn);
        //    bool alliedTurn = !isAiTurn(); // TODO check faction

        //    gUIController.StartNewTurn(alliedTurn, objectiveManager.getObjectives(currentPlayersTurn));

        //    playerManager.StartNewTurn(currentPlayersTurn);

        //    uIManager.StartTurn();

        //    // TODO Had error when unit died at turn start
        //    if (!isAiTurn()) {
        //        // TODO this should move to the new players character
        //        //if (unitManager.SelectedUnit != null) {
        //        //    cameraManager.MoveToLocation(unitManager.SelectedUnit.myNode);
        //        //}
        //    } else {
        //        StartCoroutine(aiManager.NewTurn(playersTurn));
        //    }
        //}

	}

	public void EndTurn() {
		//ChangeState(TurnPhase.TURN_ENDING);
  //      PlayerData currentPlayersTurn = GetCurrentPlayer();

  //      if (objectiveManager.CheckObjectives (currentPlayersTurn)) {
		//	MenuSystem.LoadScene (Scenes.MAIN_MENU);
		//} else {
		//	unitManager.EndTurn (currentPlayersTurn);
		//	StartNewTurn ();
		//	uIManager.EndTurn ();
		//}
	}

	public TurnPhase CurrentPhase {
		get { return currentPhase; }
		set { ChangeState(value); }
	}

	public int PlayersTurn {
		get { return playersTurn; }
		set { playersTurn = value; }
	}

    public PlayerData GetCurrentPlayer() {
        return playerManager.GetPlayer(playersTurn);
    }

	public void ChangeState(TurnPhase newPhase) {
		currentPhase = newPhase;
        checkedIfTurnShouldEnd = false;
	}

	public void FinishStartingTurn() {
		ChangeState(TurnPhase.WAITING_FOR_INPUT);
    }

	public void StartMoving() {
		ChangeState(TurnPhase.UNIT_MOVING);
	}

	public void FinishedMoving() {
		ChangeState(TurnPhase.WAITING_FOR_INPUT);
		uIManager.FinishedMoving ();
	}

	public void StartAttacking() {
		ChangeState(TurnPhase.UNIT_ATTACKING);
	}

	public void FinishedAttacking() {
		ChangeState(TurnPhase.WAITING_FOR_INPUT);
		uIManager.FinishedAttacking ();
    }

	public bool isAiTurn() {
		return GetCurrentPlayer().ai;
	}


    // NETWORKING
    ///////////////////////////////

    // Client RPCs
    ///////////////////////////////

    [ClientRpc]
    private void RpcGameState(TurnPhase newPhase, string message) {
        ClientHandleState(newPhase, message);
    }

    // SERVER
    ///////////////////////////////

    [Server]
    public void ServerStartGame() {
        gameHasStarted = true;
        Invoke("ServerNextTurn", 2.0f);
    }

    [Server]
    public void ServerEndTurn() {
        ServerEnterGameState(TurnPhase.TURN_ENDING, "Ending turn");

        ServerNextTurn();
    }

    [Server]
    public void ServerNextTurn() {
        ServerEnterGameState(TurnPhase.TURN_STARTING, "Starting new turn");

        ServerNextPlayersTurn();
    }

    [Server]
    public void ServerNextPlayersTurn() {
        // if there is currently a player turn
        if (currentTurnPlayer != null) {
            // Set the old player enabled to false
            currentTurnPlayer.RpcYourTurn(false);
        }
        playersTurn++;
        playersTurn = playersTurn % GameManager.singleton.players.Count;
        currentTurnPlayer = GameManager.singleton.players[playersTurn];
        currentTurnPlayer.RpcYourTurn(true);

        Debug.Log("Drawing cards for player " + playersTurn);
        currentTurnPlayer.CmdDrawCard();
        currentTurnPlayer.CmdDrawCard();
        currentTurnPlayer.CmdDrawCard();
        currentTurnPlayer.CmdDrawCard();
        currentTurnPlayer.CmdDrawCard();

        ServerEnterGameState(TurnPhase.WAITING_FOR_INPUT, "Waiting for player " + playersTurn + " input");
    }

    // enters the state immediately
    [Server]
    void ServerEnterGameState(TurnPhase newPhase, string message) {
        Debug.Log("Server Enter state:" + newPhase);
        currentPhase = newPhase;
        RpcGameState(newPhase, message);
    }

    // CLIENT
    ///////////////////////////////

    [Client]
    void ClientHandleState(TurnPhase newPhase, string message) {
        currentPhase = newPhase;
        string msg = "Client TurnState:" + newPhase + " : " + message;
        //infoText.text = message;
        Debug.Log(msg);

        switch (newPhase) {
            case TurnPhase.WAITING_FOR_INPUT: {
                    break;
                }
            case TurnPhase.TURN_STARTING: {
                    break;
                }
            case TurnPhase.TURN_ENDING: {
                    break;
                }
            case TurnPhase.UNIT_ATTACKING: {
                    break;
                }
            case TurnPhase.UNIT_MOVING: {
                    break;
                }
        }
    }

}
