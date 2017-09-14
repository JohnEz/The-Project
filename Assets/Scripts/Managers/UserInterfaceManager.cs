using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour {

	bool showingAbility = false;

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

	void UserControls() {
		if (turnManager.CurrentPhase == TurnPhase.WAITING_FOR_INPUT) {
			if (Input.GetKeyUp ("1")) {
				showingAbility = unitManager.ShowAbility (0);
			}

			if (Input.GetKeyUp ("2")) {
				showingAbility = unitManager.ShowAbility (1);
			}

			if (Input.GetKeyUp ("space")) {
				turnManager.EndTurn ();
			}

			if (Input.GetKeyUp (KeyCode.Escape)) {
				if (showingAbility) {
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
		showingAbility = false;
		DeselectUnit ();
	}

	public void ClickedUnselected(Node node) {
		if (node.myUnit != null) {
			if (unitManager.UnitAlreadySelected (node.myUnit)) {
				ShowMovement ();
			} else {
				SelectUnit (node.myUnit);

				if (node.myUnit.myPlayer.id == turnManager.PlayersTurn) {
					if (node.myUnit.myStats.ActionPoints > 0) {
						ShowMovement ();
					}
				}
			}
		} else if (showingAbility) {
			ShowMovement ();
		}
	}

	public void StartTurn() {
		SelectNextUnit ();
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
			showingAbility = false;
		}
	}

	public void FinishedAttacking() {
		if (!turnManager.isAiTurn ()) {
			if (!ReselectUnit ()) {
				SelectNextUnit ();
				showingAbility = false;
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
		showingAbility = false;
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
			SelectUnit (nextUnit);
			ShowMovement ();
		}
	}

	public void SelectPreviousUnit() {
		UnitController nextUnit = unitManager.GetPreviousUnit (turnManager.PlayersTurn);
		if (nextUnit != null) {
			SelectUnit (nextUnit);
			ShowMovement ();
		}
	}
}
