using UnityEngine;

public enum TurnPhase {
    GAME_STARTING,
    TURN_STARTING,
    WAITING_FOR_INPUT,
    UNIT_MOVING,
    UNIT_ATTACKING,
    TURN_ENDING,
    GAME_OVER
}

public class TurnManager : MonoBehaviour {
    public static TurnManager singleton;

    private TileMap map;

    private TurnPhase currentPhase = TurnPhase.GAME_STARTING;

    private int playersTurn = -1;

    private bool checkedIfTurnShouldEnd = true;

    private void Awake() {
        singleton = this;
    }

    // Use this for initialization
    public void Initialise(TileMap _map) {
        map = _map;
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

    public void StartNewTurn() {
        ChangeState(TurnPhase.TURN_STARTING);
        playersTurn++;
        playersTurn = playersTurn % PlayerManager.singleton.GetNumberOfPlayers();
        Player currentPlayersTurn = GetCurrentPlayer();
        UnitManager.singleton.StartTurn(currentPlayersTurn);
        bool alliedTurn = PlayerManager.singleton.mainPlayer.faction == currentPlayersTurn.faction;

        GUIController.singleton.StartNewTurn(alliedTurn, ObjectiveManager.singleton.getObjectives(currentPlayersTurn));

        PlayerManager.singleton.StartNewTurn(currentPlayersTurn);

        UserInterfaceManager.singleton.StartTurn();

        // TODO Had error when unit died at turn start
        if (isAiTurn()) {
            StartCoroutine(AIManager.singleton.NewTurn(playersTurn));
        } else if (currentPlayersTurn.myCharacter != null) {
            CameraManager.singleton.JumpToLocation(currentPlayersTurn.myCharacter.myNode);
        }
    }

    public void EndTurn() {
        ChangeState(TurnPhase.TURN_ENDING);
        Player currentPlayersTurn = GetCurrentPlayer();

        if (ObjectiveManager.singleton.CheckObjectives(currentPlayersTurn)) {
            ChangeState(TurnPhase.GAME_OVER);
            GUIController.singleton.GameOver(GetCurrentPlayer() == PlayerManager.singleton.mainPlayer);
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
}