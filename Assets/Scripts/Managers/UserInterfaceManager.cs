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
		SetUnshowAbility ();
		showingAbility = abilityIndex;
		gUIController.AbilitySelected (abilityIndex);
	}

	public void SetUnshowAbility() {
		if (isShowingAbility) {
			gUIController.AbilityDeselected (showingAbility);
			showingAbility = -1;
		}
	}

	void UserControls() {
		if (turnManager.CurrentPhase == TurnPhase.WAITING_FOR_INPUT) {
			if (Input.GetKeyUp ("1")) {
				if (unitManager.ShowAbility (0)) {
					SetShowingAbility(0);
				}
			}

			if (Input.GetKeyUp ("2")) {
				if (unitManager.ShowAbility (1)) {
					SetShowingAbility(1);
				}
			}

			if (Input.GetKeyUp ("space")) {
				turnManager.EndTurn ();
			}

			if (Input.GetKeyUp (KeyCode.Escape)) {
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
		SetUnshowAbility();
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
		} else if (isShowingAbility) {
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
			SetUnshowAbility();
		}
	}

	public void FinishedAttacking() {
		if (!turnManager.isAiTurn ()) {
			if (!ReselectUnit ()) {
				SelectNextUnit ();
				SetUnshowAbility();
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
		SetUnshowAbility();
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
