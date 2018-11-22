using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum TurnPhase {
	TURN_STARTING,
	WAITING_FOR_INPUT,
	UNIT_MOVING,
	UNIT_ATTACKING,
	TURN_ENDING
}

public class TurnManager : MonoBehaviour {

	UserInterfaceManager uIManager;
	GUIController gUIController;
	UnitManager unitManager;
	AIManager aiManager;
	CameraManager cameraManager;
	TileMap map;
	ObjectiveManager objectiveManager;
    PlayerManager playerManager;

	TurnPhase currentPhase = TurnPhase.TURN_STARTING;

	int playersTurn = -1;

	bool checkedIfTurnShouldEnd = true; 

	// Use this for initialization
	public void Initialise () {
		uIManager = GetComponentInChildren<UserInterfaceManager> ();
		gUIController = GetComponentInChildren<GUIController> ();
		map = GetComponentInChildren<TileMap> ();
		map.Initialise ();
		unitManager = GetComponent<UnitManager> ();
		unitManager.Initialise (map);
		aiManager = GetComponent<AIManager> ();
		aiManager.Initialise (map);
		cameraManager = GetComponent<CameraManager> ();
		cameraManager.Initialise ();
        playerManager = GetComponent<PlayerManager>();
        AddPlayers();
        objectiveManager = GetComponent<ObjectiveManager> ();
		objectiveManager.Initialise ();
		AddObjectives ();
		StartNewTurn ();
	}
	
	// Update is called once per frame
	void Update () {
		//check to see if the turn should end
		if (!checkedIfTurnShouldEnd) {
			if (!isAiTurn() && currentPhase == TurnPhase.WAITING_FOR_INPUT && unitManager.PlayerOutOfActions (playersTurn)) {
				EndTurn ();
			} else {
                checkedIfTurnShouldEnd = true;
			}
		}
	}

	//TEMP
	void AddPlayers() {
        playerManager.AddPlayer(1, "Jonesy");

        if (MatchDetails.VersusAi) {
            playerManager.AddAiPlayer(2);
        } else {
            playerManager.AddPlayer(2, "Jimmy");
        }

        unitManager.SpawnUnit(7, playerManager.GetPlayer(0), 16, 10);
        unitManager.SpawnUnit(2, playerManager.GetPlayer(1), 17, 10);
    }

	//TEMP
	void AddObjectives() {
		Objective objective = new Objective ();
		objective.optional = false;
		objective.text = "Kill all enemies!";
		objective.type = ObjectiveType.ANNIHILATION;
		objectiveManager.AddObjective (playerManager.GetPlayer(0), objective);

		Objective objective2 = new Objective ();
		objective2.optional = false;
		objective2.text = "Kill all enemies!";
		objective2.type = ObjectiveType.ANNIHILATION;
		objectiveManager.AddObjective (playerManager.GetPlayer(1), objective2);
	}

	public void StartNewTurn() {
		ChangeState(TurnPhase.TURN_STARTING);
		playersTurn++;
		playersTurn = playersTurn % playerManager.GetNumberOfPlayers();
        Player currentPlayersTurn = GetCurrentPlayer();
        unitManager.StartTurn (currentPlayersTurn);
		bool alliedTurn = !isAiTurn(); // TODO check faction

		gUIController.StartNewTurn (alliedTurn, objectiveManager.getObjectives(currentPlayersTurn));

        playerManager.StartNewTurn(currentPlayersTurn);

		uIManager.StartTurn ();

		// TODO Had error when unit died at turn start
		if (!isAiTurn ()) {
            // TODO this should move to the new players character
            //if (unitManager.SelectedUnit != null) {
            //    cameraManager.MoveToLocation(unitManager.SelectedUnit.myNode);
            //}
		} else {
			StartCoroutine(aiManager.NewTurn (playersTurn));
		}

	}

	public void EndTurn() {
		ChangeState(TurnPhase.TURN_ENDING);
        Player currentPlayersTurn = GetCurrentPlayer();

        if (objectiveManager.CheckObjectives (currentPlayersTurn)) {
			MenuSystem.LoadScene (Scenes.MAIN_MENU);
		} else {
			unitManager.EndTurn (currentPlayersTurn);
			StartNewTurn ();
			uIManager.EndTurn ();
		}
	}

	public TurnPhase CurrentPhase {
		get { return currentPhase; }
		set { ChangeState(value); }
	}

	public int PlayersTurn {
		get { return playersTurn; }
		set { playersTurn = value; }
	}

    public Player GetCurrentPlayer() {
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
}
