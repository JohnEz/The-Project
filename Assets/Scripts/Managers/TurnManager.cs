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
    public static TurnManager instance;

    private TurnPhase currentPhase = TurnPhase.GAME_STARTING;

    private int playersTurn = -1;

    private bool checkedIfTurnShouldEnd = true;

    private void Awake() {
        instance = this;
    }

    // Use this for initialization
    public void Initialise() {
    }

    // Update is called once per frame
    private void Update() {
        //check to see if the turn should end
        if (!checkedIfTurnShouldEnd) {
            if (!isAiTurn() && currentPhase == TurnPhase.WAITING_FOR_INPUT && UnitManager.instance.PlayerOutOfActions(playersTurn)) {
                EndTurn();
            } else {
                checkedIfTurnShouldEnd = true;
            }
        }
    }

    public void StartGame() {
        PlayerManager.instance.StartGame();

        StartNewTurn();
    }

    private Player SetNextPlayersTurn() {
        playersTurn++;
        playersTurn = playersTurn % PlayerManager.instance.GetNumberOfPlayers();
        return GetCurrentPlayer();
    }

    public void StartNewTurn() {
        ChangeState(TurnPhase.TURN_STARTING);
        int previousPlayersTurn = playersTurn;
        Player currentPlayersTurn = SetNextPlayersTurn();

        while (UnitManager.instance.GetPlayersUnits(currentPlayersTurn.id).Count <= 0 || playersTurn == previousPlayersTurn) {
            currentPlayersTurn = SetNextPlayersTurn();
        }

        UnitManager.instance.StartTurn(currentPlayersTurn);
        bool alliedTurn = PlayerManager.instance.mainPlayer.faction == currentPlayersTurn.faction;

        GUIController.instance.StartNewTurn(alliedTurn);

        PlayerManager.instance.StartNewTurn(currentPlayersTurn);

        if (isAiTurn()) {
            StartCoroutine(AIManager.instance.NewTurn(playersTurn));
        } else if (currentPlayersTurn.units.Count > 0) {
            CameraManager.instance.JumpToLocation(currentPlayersTurn.units[0].myNode);
        }
    }

    public void EndTurn() {
        ChangeState(TurnPhase.TURN_ENDING);
        PlayerManager.instance.EndTurn(GetCurrentPlayer());
        Player currentPlayersTurn = GetCurrentPlayer();

        GameOutcome gameOutcome = ObjectiveManager.instance.CheckObjectives(currentPlayersTurn);

        if (gameOutcome == GameOutcome.NONE) {
            UnitManager.instance.EndTurn(currentPlayersTurn);
            StartNewTurn();
            UserInterfaceManager.instance.EndTurn();
        } else {
            ChangeState(TurnPhase.GAME_OVER);

            // TODO we shouldnt need to check if the ai won imo
            bool isVictory = gameOutcome == GameOutcome.WIN;
            bool isPlayer = GetCurrentPlayer().faction == PlayerManager.instance.mainPlayer.faction;
            bool playerWon = (isPlayer && isVictory) || (!isPlayer && !isVictory);
            GUIController.instance.GameOver(playerWon);
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
        return PlayerManager.instance.GetPlayer(playersTurn);
    }

    public bool IsPlayersTurn() {
        return PlayerManager.instance.IsMainPlayer(playersTurn);
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
        UserInterfaceManager.instance.FinishedMoving();
    }

    public void StartAttacking() {
        ChangeState(TurnPhase.UNIT_ATTACKING);
    }

    public void FinishedAttacking() {
        // TODO check for triggers?
        ChangeState(TurnPhase.WAITING_FOR_INPUT);
        UserInterfaceManager.instance.FinishedAttacking();
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