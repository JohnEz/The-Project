using UnityEngine;
using System.Collections;

public enum TurnPhase {
	TURN_STARTING,
	WAITING_FOR_INPUT,
	UNIT_MOVING,
	UNIT_ATTACKING,
	TURN_ENDING
}

public class TurnManager : MonoBehaviour {

	UnitManager unitManager;

	TurnPhase currentPhase = TurnPhase.WAITING_FOR_INPUT;
	int playersTurn = 1;

	// Use this for initialization
	void Start () {
		unitManager = GetComponent<UnitManager> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//FINITE STATE MACHINE

	public void StartMoving() {
		currentPhase = TurnPhase.UNIT_MOVING;
	}

	public void FinishedMoving() {
		currentPhase = TurnPhase.WAITING_FOR_INPUT;
	}

	//MOVEMENT PHASE

	public void TileClicked(Node node, SquareTarget target) {
		
		if (currentPhase == TurnPhase.WAITING_FOR_INPUT) {
			
			switch (target) {
			case SquareTarget.ATTACK:
				break;
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
		StartMoving ();
		unitManager.MoveToTile (node);
		unitManager.DeselectUnit ();
	}

	public void ClickedUnselected(Node node) {
		if (!ShowMovement (node)) {
			unitManager.DeselectUnit ();
		}
	}

	public bool ShowMovement (Node node) {
		if (node.myUnit != null && node.myUnit.actionPoints > 0) {
			if (unitManager.SelectUnit (node.myUnit)) {
				unitManager.ShowMovement (node.myUnit);
			}
			return true;
		}
		return false;
	}
}
