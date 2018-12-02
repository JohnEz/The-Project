using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour {

    public static UserInterfaceManager singleton;

	//Managers
	PauseMenuController pauseMenuController;

    AbilityCardBase activeCard = null;
    int currentActionIndex = 0;
    bool cardPlayed = false;

    private void Awake() {
        singleton = this;
    }

    // Use this for initialization
    void Start () {
		////gUIController = GetComponentInChildren<GUIController> ();
		//turnManager = GetComponentInParent<TurnManager> ();
  //      //playerManager = GetComponentInParent<PlayerManager>();
  //      pauseMenuController = GetComponentInChildren<PauseMenuController> ();

  //      Debug.Log("Starting turn manager");
  //      turnManager.Initialise ();
	}
	
	// Update is called once per frame
	void Update () {
        //UserControls ();
	}

	public void ShowCard(AbilityCardBase card) {
        UnshowCard();
        activeCard = card;

        if (!cardPlayed) {
            activeCard = card;
            ShowAction();
        }
    }

	public void UnshowCard() {
        if (activeCard) {
            UnitManager.singleton.ClearMovementTiles();
        }
    }

	void UserControls() {

		////temp for ai
		//if (Input.GetKeyUp ("space")) {
		//	turnManager.EndTurn ();
		//}

  //      if (Input.GetKeyUp(KeyCode.E)) {
  //          //turnManager.GetCurrentPlayer().myDeck.DrawCard();
  //      }

		////Cancel (right click)
		//if (Input.GetKeyUp (KeyCode.Escape)) {
  //          if (cardPlayed) {
  //              cardPlayed = false;
  //              UnshowCard();
  //          } else {
  //              if (PauseMenuController.gameIsPaused) {
  //                  pauseMenuController.Resume();
  //              } else {
  //                  pauseMenuController.Pause();
  //              }
  //          }
		//}

		//if (turnManager.CurrentPhase == TurnPhase.WAITING_FOR_INPUT && !turnManager.isAiTurn()) {

		//	if (Input.GetKeyUp ("space")) {
		//		//turnManager.EndTurn ();
		//	}
		//}

	}

	public void TileHovered(Node node, SquareTarget target) {
		//unitManager.CurrentlyHoveredNode = node;
		//if (cardPlayed && (target == SquareTarget.ATTACK || target == SquareTarget.HELPFULL)) {
		//	unitManager.HighlightEffectedTiles (node);
		//} else if (target == SquareTarget.MOVEMENT || target == SquareTarget.DASH || ((target == SquareTarget.ATTACK || target == SquareTarget.HELPFULL) && node.previousMoveNode != null)) {
		//	unitManager.ShowPath (node);
		//}
	}

	public void TileExit(Node node, SquareTarget target) {
		//unitManager.CurrentlyHoveredNode = null;
		//unitManager.UnhiglightEffectedTiles ();
	}

	public void TileClicked(Node node, SquareTarget target) {

		//if (turnManager.CurrentPhase == TurnPhase.WAITING_FOR_INPUT && !turnManager.isAiTurn()) {
		//	switch (target) {
		//	case SquareTarget.HELPFULL:
		//	case SquareTarget.ATTACK:
		//		ClickedAttack (node);
		//		break;
		//	case SquareTarget.DASH:
		//	case SquareTarget.MOVEMENT:
		//		ClickedMovement (node);
		//		break;
		//	case SquareTarget.NONE: 
		//	default:
		//		ClickedUnselected (node);
		//		break;
		//	}

		//}

	}

	public void ClickedMovement(Node node) {
		//unitManager.MoveToTile (node);
		//UnshowCard();
	}

	public void ClickedUnselected(Node node) {

	}

	public void StartTurn() {
		//if (!turnManager.isAiTurn ()) {
			
		//}
	}

	public void EndTurn() {

	}

    public bool CanPlayCard() {
        return !cardPlayed;
    }

    public void CardHovered(AbilityCardBase card) {
        if (!cardPlayed) {
            ShowCard(card);
        }
    }

    public void CardUnhovered() {
        // if there isnt a played card, clear the display
        if (!cardPlayed) {
            UnshowCard();
            activeCard = null;
        }
    }

    public void CardPlayed(AbilityCardBase card) {
        //ShowCard(card);
        //cardPlayed = true;
    }

    public void ClickedAttack(Node node) {
		//if (unitManager.AttackTile (node)) {
		//	UnshowCard();
		//}
	}

    public bool ShowAction() {
        if (activeCard && currentActionIndex < activeCard.Actions.Count) {
            CardAction currentAction = activeCard.Actions[currentActionIndex];

            if (currentAction.GetType() == typeof(MoveAction)) {
                MoveAction moveAction = (MoveAction)currentAction;
                UnitManager.singleton.ShowMoveAction(moveAction.distance, moveAction.walkingType);
            } else if (typeof(AttackAction).IsAssignableFrom(currentAction.GetType())) {
                AttackAction attackAction = (AttackAction)currentAction;
                UnitManager.singleton.ShowAttackAction(attackAction);
            }
            return true;
        }
        return false;
    }

    public void FinishedAction() {
        //currentActionIndex++;
        //if (!ShowAction()) {
        //    activeCard = null;
        //    cardPlayed = false;
        //    currentActionIndex = 0;
        //}
    }

	public void FinishedAttacking() {
		//if (!turnManager.isAiTurn ()) {
  //          FinishedAction();
  //      }
	}

	public void FinishedMoving() {
		//if (!turnManager.isAiTurn ()) {
  //          FinishedAction();
  //      }
	}

	//public void ShowMovement() {
	//	UnshowAbility();
	//	unitManager.ShowActions ();
	//}

}
