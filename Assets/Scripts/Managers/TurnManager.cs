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

	TurnPhase currentPhase = TurnPhase.TURN_STARTING;

	List<Player> players;
	public int playersTurn = -1;


	// Use this for initialization
	void Start () {
		unitManager = GetComponent<UnitManager> ();
		unitManager.Initialise ();
		AddPlayers ();
		StartNewTurn ();
	}
	
	// Update is called once per frame
	void Update () {
		//TODO these should probably be moved to a interface class
		if (Input.GetKeyUp ("1") && currentPhase == TurnPhase.WAITING_FOR_INPUT) {
			unitManager.ShowAbility (0);
		}
	}

	void AddPlayers() {
		players = new List<Player> ();

		Player p1 = new Player ();
		p1.name = "Player 1";
		p1.ai = false;
		players.Add (p1);

		Player p2 = new Player ();
		p2.name = "Player 2";
		p2.ai = false;
		players.Add (p2);

		Player p3 = new Player ();
		p3.name = "AI";
		p3.ai = true;
		players.Add (p3);

	}

	//FINITE STATE MACHINE

	public void StartNewTurn() {
		currentPhase = TurnPhase.TURN_STARTING;
		playersTurn++;
		playersTurn = playersTurn % players.Count;
		GetComponent<UnitManager> ().StartTurn (playersTurn);
		GetComponentInChildren<UserInterfaceController> ().StartNewTurn (players [playersTurn].name + " turn");
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
		if (!ShowMovement (node)) {
			unitManager.DeselectUnit ();
		}
	}

	public void ClickedAttack(Node node) {
		if (unitManager.AttackTile (node)) {
			unitManager.DeselectUnit ();
			StartAttacking ();
		}
	}

	public bool ShowMovement (Node node) {
		if (node.myUnit != null && node.myUnit.myStats.ActionPoints > 0 && node.myUnit.myTeam == playersTurn) {
			if (unitManager.SelectUnit (node.myUnit)) {
				unitManager.ShowMovement (node.myUnit);
			}
			return true;
		}
		return false;
	}
}
