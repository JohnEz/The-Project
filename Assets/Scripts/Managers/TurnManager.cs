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

    public static TurnManager singleton;

	TileMap map;

	TurnPhase currentPhase = TurnPhase.TURN_STARTING;

	int playersTurn = -1;

	bool checkedIfTurnShouldEnd = true;

    private void Awake() {
        singleton = this;
    }

    // Use this for initialization
    public void Initialise(TileMap _map) {
        map = _map;
    }
	
	// Update is called once per frame
	void Update () {
		//check to see if the turn should end
		if (!checkedIfTurnShouldEnd) {
			if (!isAiTurn() && currentPhase == TurnPhase.WAITING_FOR_INPUT && UnitManager.singleton.PlayerOutOfActions (playersTurn)) {
				EndTurn ();
			} else {
                checkedIfTurnShouldEnd = true;
			}
		}
	}

    public void StartGame() {
        PlayerManager.singleton.StartGame();

        StartNewTurn();
    }

	public void StartNewTurn() {
		ChangeState(TurnPhase.TURN_STARTING);
		playersTurn++;
		playersTurn = playersTurn % PlayerManager.singleton.GetNumberOfPlayers();
        Player currentPlayersTurn = GetCurrentPlayer();
        UnitManager.singleton.StartTurn (currentPlayersTurn);
		bool alliedTurn = !isAiTurn(); // TODO check faction

		GUIController.singleton.StartNewTurn (alliedTurn, ObjectiveManager.singleton.getObjectives(currentPlayersTurn));

        PlayerManager.singleton.StartNewTurn(currentPlayersTurn);

		UserInterfaceManager.singleton.StartTurn ();

		// TODO Had error when unit died at turn start
		if (!isAiTurn ()) {
            // TODO this should move to the new players character
            //if (unitManager.SelectedUnit != null) {
            //    cameraManager.MoveToLocation(unitManager.SelectedUnit.myNode);
            //}
		} else {
			StartCoroutine(AIManager.singleton.NewTurn (playersTurn));
		}

	}

	public void EndTurn() {
		ChangeState(TurnPhase.TURN_ENDING);
        Player currentPlayersTurn = GetCurrentPlayer();

        if (ObjectiveManager.singleton.CheckObjectives (currentPlayersTurn)) {
			MenuSystem.LoadScene (Scenes.MAIN_MENU);
		} else {
			UnitManager.singleton.EndTurn (currentPlayersTurn);
			StartNewTurn ();
			UserInterfaceManager.singleton.EndTurn ();
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
        return PlayerManager.singleton.GetPlayer(playersTurn);
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
		UserInterfaceManager.singleton.FinishedMoving ();
	}

	public void StartAttacking() {
		ChangeState(TurnPhase.UNIT_ATTACKING);
	}

	public void FinishedAttacking() {
		ChangeState(TurnPhase.WAITING_FOR_INPUT);
        UserInterfaceManager.singleton.FinishedAttacking ();
    }

	public bool isAiTurn() {
		return GetCurrentPlayer().ai;
	}
}
