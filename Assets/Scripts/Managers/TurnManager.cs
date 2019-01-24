using System.Collections;
using UnityEngine;

public enum TurnPhase {
    GAME_STARTING,
    TURN_STARTING,
    WAITING_FOR_INPUT,
    UNIT_MOVING,
    UNIT_ATTACKING,
    TURN_ENDING,
    CUTSCENE,
    GAME_OVER
}

public class TurnManager : MonoBehaviour {
    public static TurnManager singleton;

    private TurnPhase currentPhase = TurnPhase.GAME_STARTING;

    private int playersTurn = -1;

    private bool checkedIfTurnShouldEnd = true;

    private void Awake() {
        singleton = this;
    }

    // Use this for initialization
    public void Initialise() {
    }

    // Update is called once per frame
    private void Update() {
        //check to see if the turn should end
        if (!checkedIfTurnShouldEnd) {
            if (!isAiTurn() && currentPhase == TurnPhase.WAITING_FOR_INPUT && UnitManager.singleton.PlayerOutOfActions(playersTurn)) {
                EndTurn();
            } else {
                checkedIfTurnShouldEnd = true;
            }
        }
    }

    public void StartGame() {
        PlayerManager.singleton.StartGame();

        StartNewTurn();
    }

    private Player SetNextPlayersTurn() {
        playersTurn++;
        playersTurn = playersTurn % PlayerManager.singleton.GetNumberOfPlayers();
        return GetCurrentPlayer();
    }

    public void StartNewTurn() {
        ChangeState(TurnPhase.TURN_STARTING);
        int previousPlayersTurn = playersTurn;
        Player currentPlayersTurn = SetNextPlayersTurn();

        while (UnitManager.singleton.GetPlayersUnits(currentPlayersTurn.id).Count <= 0 || playersTurn == previousPlayersTurn) {
            currentPlayersTurn = SetNextPlayersTurn();
        }
        
        UnitManager.singleton.StartTurn(currentPlayersTurn);
        bool alliedTurn = PlayerManager.singleton.mainPlayer.faction == currentPlayersTurn.faction;

        GUIController.singleton.StartNewTurn(alliedTurn, ObjectiveManager.singleton.getObjectives(currentPlayersTurn));

        PlayerManager.singleton.StartNewTurn(currentPlayersTurn);

        UserInterfaceManager.singleton.StartTurn();

        if (isAiTurn()) {
            StartCoroutine(AIManager.singleton.NewTurn(playersTurn));
        } else if (currentPlayersTurn.units.Count > 0) {
            CameraManager.singleton.JumpToLocation(currentPlayersTurn.units[0].unit.myNode);
        }
    }

    public void EndTurn() {
        ChangeState(TurnPhase.TURN_ENDING);
        Player currentPlayersTurn = GetCurrentPlayer();

        if (ObjectiveManager.singleton.CheckObjectives(currentPlayersTurn)) {
            ChangeState(TurnPhase.GAME_OVER);
            GUIController.singleton.GameOver(GetCurrentPlayer().faction == PlayerManager.singleton.mainPlayer.faction);
        } else {
            UnitManager.singleton.EndTurn(currentPlayersTurn);
            StartNewTurn();
            UserInterfaceManager.singleton.EndTurn();
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
        UserInterfaceManager.singleton.FinishedMoving();
    }

    public void StartAttacking() {
        ChangeState(TurnPhase.UNIT_ATTACKING);
    }

    public void FinishedAttacking() {
        // TODO check for triggers?
        ChangeState(TurnPhase.WAITING_FOR_INPUT);
        UserInterfaceManager.singleton.FinishedAttacking();
    }

    public bool isAiTurn() {
        return GetCurrentPlayer().ai;
    }

    public IEnumerator WaitForWaitingForInput() {
        return new WaitUntil(() => CurrentPhase == TurnPhase.WAITING_FOR_INPUT);
    }

    public void StartingCutscene() {
        CurrentPhase = TurnPhase.CUTSCENE;
    }

    public void EndedCutscene() {
        // TODO this maybe should go back to the previous phase instead
        CurrentPhase = TurnPhase.WAITING_FOR_INPUT;
    }
}