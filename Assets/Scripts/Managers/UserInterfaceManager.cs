using UnityEngine;

public enum CardState {
    NONE,
    PLAYED,
    INVOKED
}

public class UserInterfaceManager : MonoBehaviour {
    public static UserInterfaceManager singleton;

    //Managers
    private PauseMenuController pauseMenuController;

    private CardSlot activeCard = null;
    private int currentActionIndex = 0;
    private CardState cardState = CardState.NONE;

    private void Awake() {
        singleton = this;
    }

    // Use this for initialization
    private void Start() {
        pauseMenuController = GetComponent<PauseMenuController>();
    }

    // Update is called once per frame
    private void Update() {
        UserControls();
    }

    public void ShowCard(CardSlot cardSlot) {
        UnshowCard();
        activeCard = cardSlot;

        if (cardState == CardState.NONE) {
            ShowAction();
        }
    }

    public void UnshowCard() {
        if (activeCard) {
            HighlightManager.singleton.UnhighlightTiles();
            HighlightManager.singleton.ClearEffectedTiles();
        }
    }

    // When the user clicked an action from the card
    public void CardInvoked() {
        cardState = CardState.INVOKED;
    }

    private void UserControls() {
        //temp for ai
        //if (Input.GetKeyUp ("space")) {
        //	TurnManager.singleton.EndTurn ();
        //}

        //Cancel (right click)
        if (Input.GetKeyUp(KeyCode.Escape)) {
            if (cardState == CardState.PLAYED) {
                CancelCurrentCard();
            } else {
                if (PauseMenuController.gameIsPaused) {
                    pauseMenuController.Resume();
                } else {
                    pauseMenuController.Pause();
                }
            }
        }

        if (TurnManager.singleton.CurrentPhase == TurnPhase.WAITING_FOR_INPUT && !TurnManager.singleton.isAiTurn()) {
            if (Input.GetKeyUp("space")) {
                if (cardState == CardState.PLAYED) {
                    CancelCurrentCard();
                }
                TurnManager.singleton.EndTurn();
            }
        }
    }

    public void CancelCurrentCard() {
        if (cardState == CardState.PLAYED) {
            cardState = CardState.NONE;
            activeCard.CancelCard();
            //PlayerManager.singleton.mainPlayer.CurrentInfluence += activeCard.ability.staminaCost;
            activeCard.card.caster.Stamina += activeCard.card.staminaCost;
            UnshowCard();
        }
    }

    public void TileHovered(Node node, SquareTarget target) {
        UnitManager.singleton.CurrentlyHoveredNode = node;
        if (cardState != CardState.NONE && (target == SquareTarget.ATTACK || target == SquareTarget.HELPFULL)) {
            UnitManager.singleton.HighlightEffectedTiles(activeCard.card.caster, node);
        } else if (target == SquareTarget.MOVEMENT || target == SquareTarget.DASH || ((target == SquareTarget.ATTACK || target == SquareTarget.HELPFULL) && node.previousMoveNode != null)) {
            UnitManager.singleton.ShowPath(node);
        }
    }

    public void TileExit(Node node, SquareTarget target) {
        UnitManager.singleton.CurrentlyHoveredNode = null;
        UnitManager.singleton.UnhiglightEffectedTiles();
    }

    public void TileClicked(Node node, SquareTarget target) {
        if (TurnManager.singleton.CurrentPhase == TurnPhase.WAITING_FOR_INPUT && !TurnManager.singleton.isAiTurn()) {
            switch (target) {
                case SquareTarget.HELPFULL:
                case SquareTarget.ATTACK:
                    ClickedAttack(node);
                    break;

                case SquareTarget.DASH:
                case SquareTarget.MOVEMENT:
                    ClickedMovement(node);
                    break;

                case SquareTarget.NONE:
                default:
                    ClickedUnselected(node);
                    break;
            }
        }
    }

    public void ClickedAttack(Node node) {
        if (UnitManager.singleton.AttackTile(activeCard.card.caster, node)) {
            UnshowCard();
            CardInvoked();
        }
    }

    public void ClickedMovement(Node node) {
        UnitManager.singleton.MoveToTile(activeCard.card.caster, node);
        UnshowCard();
        CardInvoked();
    }

    public void ClickedUnselected(Node node) {
    }

    public void StartTurn() {
        if (!TurnManager.singleton.isAiTurn()) {
        }
    }

    public void EndTurn() {
    }

    public bool CanPlayCard() {
        return
            cardState == CardState.NONE &&
            TurnManager.singleton.CurrentPhase == TurnPhase.WAITING_FOR_INPUT &&
            TurnManager.singleton.GetCurrentPlayer() == PlayerManager.singleton.mainPlayer;
    }

    public bool CanPlayCard(CardSlot cardSlot) {
        return CanPlayCard() && cardSlot.card.caster.Stamina >= cardSlot.card.staminaCost;
    }

    public void CardHovered(CardSlot card) {
        if (CanPlayCard()) {
            ShowCard(card);
        }
    }

    public void CardUnhovered() {
        // if there isnt a played card, clear the display
        if (cardState == CardState.NONE) {
            UnshowCard();
            activeCard = null;
        }
    }

    public bool PlayCard(CardSlot cardSlot) {
        if (CanPlayCard(cardSlot)) {
            UnshowCard();
            cardState = CardState.PLAYED;
            cardSlot.card.caster.Stamina -= cardSlot.card.staminaCost;
            activeCard = cardSlot;
            RunNextCardAction();
            return true;
        }
        return false;
    }

    public bool RunNextCardAction() {
        if (activeCard && currentActionIndex < activeCard.card.Actions.Count) {
            CardAction currentAction = activeCard.card.Actions[currentActionIndex];
            if (currentAction.GetType() == typeof(MoveAction) || typeof(AttackAction).IsAssignableFrom(currentAction.GetType())) {
                ShowAction();
            } else if (currentAction.GetType() == typeof(DrawCardAction)) {
                DrawCardAction drawAction = (DrawCardAction)currentAction;
                activeCard.myUnit.deck.DrawCard(drawAction.cardsToDraw);
                FinishedAction();
            }
            return true;
        }

        return false;
    }

    public void ShowAction() {
        CardAction currentAction = activeCard.card.Actions[currentActionIndex];
        if (currentAction.GetType() == typeof(MoveAction)) {
            MoveAction moveAction = (MoveAction)currentAction;
            UnitManager.singleton.ShowMoveAction(activeCard.card.caster, moveAction.distance, moveAction.walkingType);
        } else if (typeof(AttackAction).IsAssignableFrom(currentAction.GetType())) {
            AttackAction attackAction = (AttackAction)currentAction;
            UnitManager.singleton.ShowAttackAction(activeCard.card.caster, attackAction);
        }
    }

    public void FinishedAction() {
        currentActionIndex++;
        if (!RunNextCardAction()) {
            Destroy(activeCard.gameObject);
            activeCard = null;
            cardState = CardState.NONE;
            currentActionIndex = 0;
        }
    }

    public void FinishedAttacking() {
        if (!TurnManager.singleton.isAiTurn()) {
            FinishedAction();
        }
    }

    public void FinishedMoving() {
        if (!TurnManager.singleton.isAiTurn()) {
            FinishedAction();
        }
    }
}