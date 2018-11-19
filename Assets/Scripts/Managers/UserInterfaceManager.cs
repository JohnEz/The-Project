using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour {

	int showingAbility = -1;

	//Managers
	TurnManager turnManager;
	UnitManager unitManager;
	GUIController gUIController;
	PauseMenuController pauseMenuController;

	// Use this for initialization
	void Start () {
		gUIController = GetComponentInChildren<GUIController> ();
		turnManager = GetComponentInParent<TurnManager> ();
		unitManager = GetComponentInParent<UnitManager> ();
		pauseMenuController = GetComponentInChildren<PauseMenuController> ();
		turnManager.Initialise ();
	}
	
	// Update is called once per frame
	void Update () {
		UserControls ();
	}

	public bool isShowingAbility {
		get { return showingAbility > -1; }
	}

	public void SetShowingAbility(int abilityIndex) {
		UnshowAbility ();
		showingAbility = abilityIndex;
		gUIController.AbilitySelected (abilityIndex);
	}

	public void UnshowAbility() {
		if (isShowingAbility) {
			gUIController.AbilityDeselected (showingAbility);
			showingAbility = -1;
		}
	}

	//public void ShowAbility(int index) {
	//	if (unitManager.ShowAbility (index)) {
	//		SetShowingAbility(index);
	//	}
	//}

	void UserControls() {

		//temp for ai
		if (Input.GetKeyUp ("space")) {
			turnManager.EndTurn ();
		}

		//Cancel (right click)
		if (Input.GetKeyUp (KeyCode.Escape) && !isShowingAbility) {
			if (PauseMenuController.gameIsPaused) {
				pauseMenuController.Resume ();
			} else {
				pauseMenuController.Pause ();
			}
		}

		if (turnManager.CurrentPhase == TurnPhase.WAITING_FOR_INPUT && !turnManager.isAiTurn()) {

			if (Input.GetKeyUp ("space")) {
				//turnManager.EndTurn ();
			}
		}

	}

	public void TileHovered(Node node, SquareTarget target) {
		unitManager.CurrentlyHoveredNode = node;
		if (isShowingAbility && (target == SquareTarget.ATTACK || target == SquareTarget.HELPFULL)) {
			unitManager.HighlightEffectedTiles (node);
		} else if (target == SquareTarget.MOVEMENT || target == SquareTarget.DASH || ((target == SquareTarget.ATTACK || target == SquareTarget.HELPFULL) && node.previousMoveNode != null)) {
			unitManager.ShowPath (node);
		}
	}

	public void TileExit(Node node, SquareTarget target) {
		unitManager.CurrentlyHoveredNode = null;
		unitManager.UnhiglightEffectedTiles ();
	}

	public void TileClicked(Node node, SquareTarget target) {

		if (turnManager.CurrentPhase == TurnPhase.WAITING_FOR_INPUT && !turnManager.isAiTurn()) {
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

	public void ClickedMovement(Node node) {
		unitManager.MoveToTile (node);
		UnshowAbility();
		DeselectUnit ();
	}

	public void ClickedUnselected(Node node) {

	}

	public void StartTurn() {
		if (!turnManager.isAiTurn ()) {
			
		}
	}

	public void EndTurn() {

	}

	public void SelectUnit(UnitController unit) {
		DeselectUnit();
		unitManager.SelectUnit (unit);
		gUIController.UnitSelected (unit);
	}

	public void DeselectUnit () {
		unitManager.DeselectUnit ();
		gUIController.UnitDeselected ();
	}

	public void ClickedAttack(Node node) {
		if (unitManager.AttackTile (node)) {
			DeselectUnit ();
			UnshowAbility();
		}
	}

	public void FinishedAttacking() {
		if (!turnManager.isAiTurn ()) {

		}
	}

	public void FinishedMoving() {
		if (!turnManager.isAiTurn ()) {

		}
	}

	//public void ShowMovement() {
	//	UnshowAbility();
	//	unitManager.ShowActions ();
	//}

}
