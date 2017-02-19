using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TurnPhase {
	TURN_STARTING,
	WAITING_FOR_INPUT,
	UNIT_MOVING,
	UNIT_ATTACKING,
	TURN_ENDING
}

public struct Player {
	public string name;
	public bool ai;
}

public class TurnManager : MonoBehaviour {

	UnitManager unitManager;

	CameraManager cameraManager;

	TurnPhase currentPhase = TurnPhase.TURN_STARTING;

	List<Player> players;
	public int playersTurn = -1;


	// Use this for initialization
	void Start () {
		unitManager = GetComponent<UnitManager> ();
		unitManager.Initialise ();
		cameraManager = GetComponent<CameraManager> ();
		cameraManager.Initialise ();
		AddPlayers ();
		StartNewTurn ();
	}
	
	// Update is called once per frame
	void Update () {
		//TODO these should probably be moved to a interface class
		if (Input.GetKeyUp ("1") && currentPhase == TurnPhase.WAITING_FOR_INPUT) {
			unitManager.ShowAbility (0);
		}

		if (Input.GetKeyUp ("space") && currentPhase == TurnPhase.WAITING_FOR_INPUT) {
			EndTurn ();
		}

		//check to see if the turn should end
		if (currentPhase == TurnPhase.WAITING_FOR_INPUT && unitManager.PlayerOutOfActions (playersTurn)) {
			EndTurn ();
		}
	}

	void AddPlayers() {
		players = new List<Player> ();

		Player p1 = new Player ();
		p1.name = "Player 1";
		p1.ai = false;
		players.Add (p1);

		Player p2 = new Player ();
		p2.name = "AI";
		p2.ai = true;
		players.Add (p2);

	}

	//FINITE STATE MACHINE

	public void StartNewTurn() {
		currentPhase = TurnPhase.TURN_STARTING;
		playersTurn++;
		playersTurn = playersTurn % players.Count;
		GetComponent<UnitManager> ().StartTurn (playersTurn);
		bool alliedTurn = players [playersTurn].name.Equals ("Player 1");

		GetComponentInChildren<UserInterfaceController> ().StartNewTurn (alliedTurn);
	}

	public void FinishStartingTurn() {
		currentPhase = TurnPhase.WAITING_FOR_INPUT;
	}

	public void StartMoving() {
		currentPhase = TurnPhase.UNIT_MOVING;
	}

	public void FinishedMoving() {
		currentPhase = TurnPhase.WAITING_FOR_INPUT;
	}

	public void StartAttacking() {
		currentPhase = TurnPhase.UNIT_ATTACKING;
	}

	public void FinishedAttacking() {
		currentPhase = TurnPhase.WAITING_FOR_INPUT;
	}

	public void EndTurn() {
		currentPhase = TurnPhase.TURN_ENDING;
		//TODO CLEAN UP, EG SELECTED TILES
		unitManager.DeselectUnit();
		StartNewTurn ();
	}

	//MOVEMENT PHASE

	public void TileClicked(Node node, SquareTarget target) {
		
		if (currentPhase == TurnPhase.WAITING_FOR_INPUT) {
			
			switch (target) {
			case SquareTarget.ATTACK:
				ClickedAttack (node);
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
		if (node.myUnit != null && !unitManager.UnitAlreadySelected(node.myUnit)) {
			unitManager.DeselectUnit ();
			unitManager.SelectUnit (node.myUnit);

			if (node.myUnit.myTeam == playersTurn) {
				if (!node.myUnit.myStats.HasMoved) {
					unitManager.ShowMovement (node.myUnit);
				} else if (node.myUnit.myStats.ActionPoints > 0) {
					unitManager.ShowAbility (0);
				}
			}
		} else {
			unitManager.DeselectUnit ();
		}
	}

	public void ClickedAttack(Node node) {
		if (unitManager.AttackTile (node)) {
			unitManager.DeselectUnit ();
			StartAttacking ();
		}
	}

	public bool doesNodeContainPlayerUnit(Node node) {
		return node.myUnit != null && node.myUnit.myTeam == playersTurn;
	}
}
