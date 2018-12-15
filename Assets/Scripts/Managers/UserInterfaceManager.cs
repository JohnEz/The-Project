using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum CardState {
    NONE,
    PLAYED,
    INVOKED
}

public class UserInterfaceManager : MonoBehaviour {

    public static UserInterfaceManager singleton;

	//Managers
	PauseMenuController pauseMenuController;

    CardDisplay activeCard = null;
    int currentActionIndex = 0;
    CardState cardState = CardState.NONE;

    private void Awake() {
        singleton = this;
    }

    // Use this for initialization
    void Start () {
        pauseMenuController = GetComponent<PauseMenuController>();
	}
	
	// Update is called once per frame
	void Update () {
		UserControls ();
	}

	public void ShowCard(CardDisplay card) {
		UnshowCard ();
        activeCard = card;

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

	void UserControls() {

		//temp for ai
		//if (Input.GetKeyUp ("space")) {
		//	TurnManager.singleton.EndTurn ();
		//}

		//Cancel (right click)
		if (Input.GetKeyUp (KeyCode.Escape)) {
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

			if (Input.GetKeyUp ("space")) {
                TurnManager.singleton.EndTurn();
            }
		}

	}

    public void CancelCurrentCard() {
        cardState = CardState.NONE;
        activeCard.gameObject.SetActive(true);
        PlayerManager.singleton.mainPlayer.myCharacter.Stamina += activeCard.ability.staminaCost;
        UnshowCard();
    }

	public void TileHovered(Node node, SquareTarget target) {
		UnitManager.singleton.CurrentlyHoveredNode = node;
		if (cardState != CardState.NONE && (target == SquareTarget.ATTACK || target == SquareTarget.HELPFULL)) {
			UnitManager.singleton.HighlightEffectedTiles (activeCard.ability.caster, node);
		} else if (target == SquareTarget.MOVEMENT || target == SquareTarget.DASH || ((target == SquareTarget.ATTACK || target == SquareTarget.HELPFULL) && node.previousMoveNode != null)) {
			UnitManager.singleton.ShowPath (node);
		}
	}

	public void TileExit(Node node, SquareTarget target) {
		UnitManager.singleton.CurrentlyHoveredNode = null;
		UnitManager.singleton.UnhiglightEffectedTiles ();
	}

	public void TileClicked(Node node, SquareTarget target) {

		if (TurnManager.singleton.CurrentPhase == TurnPhase.WAITING_FOR_INPUT && !TurnManager.singleton.isAiTurn()) {
			switch (target) {
			case SquareTarget.HELPFULL:
			case SquareTarget.ATTACK:
				ClickedAttack (node);
				break;
			case SquareTarget.DASH:
			case SquareTarget.MOVEMENT:
				ClickedMovement (node);
				break;
			case SquareTarget.NONE: 
			default:
				ClickedUnselected (node);
				break;
			}

		}

	}

    public void ClickedAttack(Node node) {
        if (UnitManager.singleton.AttackTile(activeCard.ability.caster, node)) {
            UnshowCard();
            CardInvoked();
        }
    }

    public void ClickedMovement(Node node) {
		UnitManager.singleton.MoveToTile (activeCard.ability.caster, node);
		UnshowCard();
        CardInvoked();
    }

	public void ClickedUnselected(Node node) {

	}

	public void StartTurn() {
		if (!TurnManager.singleton.isAiTurn ()) {
			
		}
	}

	public void EndTurn() {

	}

    public bool CanPlayCard() {
        return cardState == CardState.NONE;
    }

    public void CardHovered(CardDisplay card) {
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

    public void CardPlayed(CardDisplay card) {
        UnshowCard();
        cardState = CardState.PLAYED;
        TurnManager.singleton.GetCurrentPlayer().myCharacter.Stamina -= card.ability.staminaCost;
        activeCard = card;
        RunNextCardAction();
    }

    public bool RunNextCardAction() {
        if (activeCard && currentActionIndex < activeCard.ability.Actions.Count) {
            CardAction currentAction = activeCard.ability.Actions[currentActionIndex];
            if (currentAction.GetType() == typeof(MoveAction) || typeof(AttackAction).IsAssignableFrom(currentAction.GetType())) {
                ShowAction();
            } else if (currentAction.GetType() == typeof(DrawCardAction)) {
                DrawCardAction drawAction = (DrawCardAction)currentAction;
                activeCard.myPlayer.myDeck.DrawCard(drawAction.cardsToDraw);
                FinishedAction();
            }
            return true;
        }

        return false;
    }

    public void ShowAction() {
        CardAction currentAction = activeCard.ability.Actions[currentActionIndex];
        if (currentAction.GetType() == typeof(MoveAction)) {
            MoveAction moveAction = (MoveAction)currentAction;
            UnitManager.singleton.ShowMoveAction(activeCard.ability.caster, moveAction.distance, moveAction.walkingType);
        } else if (typeof(AttackAction).IsAssignableFrom(currentAction.GetType())) {
            AttackAction attackAction = (AttackAction)currentAction;
            UnitManager.singleton.ShowAttackAction(activeCard.ability.caster, attackAction);
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
		if (!TurnManager.singleton.isAiTurn ()) {
            FinishedAction();
        }
	}

	public void FinishedMoving() {
		if (!TurnManager.singleton.isAiTurn ()) {
            FinishedAction();
        }
	}

}
