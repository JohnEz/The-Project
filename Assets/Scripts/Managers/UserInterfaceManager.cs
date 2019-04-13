using UnityEngine;
using System.Collections.Generic;

public class UserInterfaceManager : MonoBehaviour {
    public static UserInterfaceManager singleton;

    //Managers
    private PauseMenuController pauseMenuController;

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

    //public void ShowCard(CardSlot cardSlot) {
    //    UnshowCard();

    //    if (!CardManager.singleton.IsACardActive()) {
    //        CardManager.singleton.ActiveCard = cardSlot;
    //        ShowAction();
    //    }
    //}

    //public void UnshowCard() {
    //    if (CardManager.singleton.ActiveCard != null) {
    //        HighlightManager.singleton.UnhighlightTiles();
    //        HighlightManager.singleton.ClearEffectedTiles();
    //    }
    //}

    private void UserControls() {
        //temp for ai
        //if (Input.GetKeyUp ("space")) {
        //	TurnManager.singleton.EndTurn ();
        //}

        //Cancel (right click)
        if (Input.GetKeyUp(KeyCode.Escape)) {
            //if (CardManager.singleton.IsACardPlayed()) {
            //    CardManager.singleton.CancelCurrentCard();
            //} else if (CardManager.singleton.IsACardInvoked()) {
            //    //check to see if they have an available option
            //    if (ActionHasAvailableOptions()) {
            //        GUIController.singleton.ShowErrorMessage("You have available options!");
            //    } else {
            //        UnshowCard();
            //        CardManager.singleton.FinishedPlayingCard();
            //    }
            //} else {
            if (PauseMenuController.gameIsPaused) {
                pauseMenuController.Resume();
            } else {
                pauseMenuController.Pause();
            }
            //}
        }

        if (Input.GetKeyUp("space")) {
            EndTurn();
        }
    }

    public void EndTurn() {
        if (TurnManager.singleton.CurrentPhase == TurnPhase.WAITING_FOR_INPUT && !TurnManager.singleton.isAiTurn()) {
            //if (CardManager.singleton.IsACardPlayed()) {
            //    CardManager.singleton.CancelCurrentCard();
            //}
            TurnManager.singleton.EndTurn();
        }
    }

    public void TileHovered(Node node, SquareTarget target) {
        UnitManager.singleton.CurrentlyHoveredNode = node;
        //if (CardManager.singleton.IsACardActive() && (target == SquareTarget.ATTACK || target == SquareTarget.HELPFULL)) {
        //    UnitManager.singleton.HighlightEffectedTiles(CardManager.singleton.ActiveCard.card.caster, node);
        //} else
        if (target == SquareTarget.MOVEMENT || target == SquareTarget.DASH || ((target == SquareTarget.ATTACK || target == SquareTarget.HELPFULL) && node.previousMoveNode != null)) {
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
        //if (UnitManager.singleton.AttackTile(CardManager.singleton.ActiveCard.card.caster, node)) {
        //    UnshowCard();
        //    CardManager.singleton.CardInvoked();
        //}
    }

    public void ClickedMovement(Node node) {
        //UnitManager.singleton.MoveToTile(CardManager.singleton.ActiveCard.card.caster, node);
        //UnshowCard();
        //CardManager.singleton.CardInvoked();
    }

    public void ClickedUnselected(Node node) {
    }

    //public void CardHovered(CardSlot card) {
    //    if (CardManager.singleton.CanPlayCard()) {
    //        ShowCard(card);
    //    }
    //}

    //public void CardUnhovered() {
    //    // if there isnt a played card, clear the display
    //    if (!CardManager.singleton.IsACardActive()) {
    //        UnshowCard();
    //        CardManager.singleton.ActiveCard = null;
    //    }
    //}

    //public bool PlayCard(CardSlot cardSlot) {
    //    if (CardManager.singleton.CanPlayCard(cardSlot)) {
    //        UnshowCard();
    //        CardManager.singleton.PlayCard(cardSlot);
    //        return true;
    //    }
    //    return false;
    //}

    //public bool RunNextCardAction(CardSlot card, int actionIndex) {
    //    if (card && actionIndex < card.card.Actions.Count) {
    //        AbilityAction currentAction = card.card.Actions[actionIndex];
    //        if (currentAction.GetType() == typeof(MoveAction) || typeof(AttackAction).IsAssignableFrom(currentAction.GetType())) {
    //            ShowAction();
    //        } else if (currentAction.GetType() == typeof(DrawCardAction)) {
    //            DrawCardAction drawAction = (DrawCardAction)currentAction;
    //            CardManager.singleton.ActiveCard.myUnit.deck.DrawCard(drawAction.cardsToDraw);
    //            FinishedAction();
    //        }
    //        return true;
    //    }

    //    return false;
    //}

    public void ShowAction() {
        //CardSlot activeCard = CardManager.singleton.ActiveCard;
        //AbilityAction currentAction = activeCard.card.Actions[CardManager.singleton.CurrentActionIndex];
        //if (currentAction.GetType() == typeof(MoveAction)) {
        //    MoveAction moveAction = (MoveAction)currentAction;
        //    UnitManager.singleton.ShowMoveAction(activeCard.card.caster, moveAction.distance, moveAction.walkingType);
        //} else if (typeof(AttackAction).IsAssignableFrom(currentAction.GetType())) {
        //    AttackAction attackAction = (AttackAction)currentAction;
        //    UnitManager.singleton.ShowAttackAction(activeCard.card.caster, attackAction);
        //}
    }

    // if there is another action to the ability, display it
    public bool ActionHasAvailableOptions() {
        //CardSlot activeCard = CardManager.singleton.ActiveCard;
        //AbilityAction currentAction = activeCard.card.Actions[CardManager.singleton.CurrentActionIndex];
        //UnitController unit = activeCard.card.caster;
        //if (currentAction.GetType() == typeof(MoveAction)) {
        //    MoveAction moveAction = (MoveAction)currentAction;
        //    ReachableTiles walkingTiles = TileMap.instance.pathfinder.findReachableTiles(unit.myNode, moveAction.distance, moveAction.walkingType, unit.myPlayer.faction);
        //    return walkingTiles.basic.Keys.Count > 0;
        //} else if (typeof(AttackAction).IsAssignableFrom(currentAction.GetType())) {
        //    AttackAction attackAction = (AttackAction)currentAction;
        //    //UnitManager.singleton.ShowAttackAction(activeCard.card.caster, attackAction);
        //    List<Node> attackableTiles = TileMap.instance.pathfinder.FindAttackableTiles(unit.myNode, attackAction);
        //    return attackableTiles.Exists(tile => attackAction.CanHitUnit(tile));
        //}

        return false;
    }

    public void FinishedAction() {
        //CardManager.singleton.CurrentActionIndex++;
        //if (!RunNextCardAction(CardManager.singleton.ActiveCard, CardManager.singleton.CurrentActionIndex)) {
        //    CardManager.singleton.FinishedPlayingCard();
        //}
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