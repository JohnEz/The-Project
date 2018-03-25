using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour {

	int showingAbility = -1;

	//Managers
	TurnManager turnManager;
	UnitManager unitManager;
	GUIController gUIController;

	// Use this for initialization
	void Start () {
		gUIController = GetComponentInChildren<GUIController> ();
		turnManager = GetComponentInParent<TurnManager> ();
		unitManager = GetComponentInParent<UnitManager> ();
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

	public void ShowAbility(int index) {
		if (unitManager.ShowAbility (index)) {
			SetShowingAbility(index);
		}
	}

	void UserControls() {

		if (Input.GetKeyUp ("space")) {
			turnManager.EndTurn ();
		}

		if (turnManager.CurrentPhase == TurnPhase.WAITING_FOR_INPUT && !turnManager.isAiTurn()) {
			if (Input.GetKeyUp ("1")) {
				ShowAbility (0);
			}

			if (Input.GetKeyUp ("2")) {
				ShowAbility (1);
			}

			if (Input.GetKeyUp ("space")) {
				//turnManager.EndTurn ();
			}

			//Cancel (right click)
			if (Input.GetKeyUp (KeyCode.Escape) || Input.GetMouseButtonUp(1)) {
				if (isShowingAbility) {
					ShowMovement ();
				}
			}

			if (Input.GetKeyUp (KeyCode.Tab)) {
				SelectNextUnit ();
			}

			if (Input.GetKeyUp(KeyCode.LeftShift)) {
				SelectPreviousUnit ();
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
		if (node.myUnit != null) {
			if (unitManager.UnitAlreadySelected (node.myUnit) && node.myUnit.ActionPoints > 0) {
				ShowMovement ();
			} else {
				SelectUnit (node.myUnit);

				if (node.myUnit.myPlayer.id == turnManager.PlayersTurn) {
					if (node.myUnit.myStats.ActionPoints > 0) {
						ShowMovement ();
					}
				}
			}
		} else if (isShowingAbility) {
			ShowMovement ();
		}
	}

	public void StartTurn() {
		if (!turnManager.isAiTurn ()) {
			SelectNextUnit ();
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
			if (!ReselectUnit ()) {
				SelectNextUnit ();
				UnshowAbility();
			}
		}
	}

	public void FinishedMoving() {
		if (!turnManager.isAiTurn ()) {
			if (unitManager.lastSelectedUnit != null && !unitManager.lastSelectedUnit.HasRemainingQueuedActions ()) {
				if (!ReselectUnit ()) {
					SelectNextUnit ();
				}
			}
		}
	}

	public void ShowMovement() {
		UnshowAbility();
		unitManager.ShowActions ();
	}

	public bool ReselectUnit() {
		if (unitManager.lastSelectedUnit != null && unitManager.lastSelectedUnit.ActionPoints > 0) {
			SelectUnit (unitManager.lastSelectedUnit);
			ShowMovement ();
			return true;
		}
		return false;
	}

	public void SelectNextUnit() {
		UnitController nextUnit = unitManager.GetNextUnit (turnManager.PlayersTurn);
		if (nextUnit != null) {
			GetComponent<CameraManager> ().MoveToLocation (nextUnit.myNode);
			SelectUnit (nextUnit);
			ShowMovement ();
		}
	}

	public void SelectPreviousUnit() {
		UnitController nextUnit = unitManager.GetPreviousUnit (turnManager.PlayersTurn);
		if (nextUnit != null) {
			GetComponent<CameraManager> ().MoveToLocation (nextUnit.myNode);
			SelectUnit (nextUnit);
			ShowMovement ();
		}
	}
}
