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
	public int id;
	public string name;
	public bool ai;
	public int faction;
}

public class TurnManager : MonoBehaviour {

	UnitManager unitManager;
	AIManager aiManager;
	CameraManager cameraManager;
	TileMap map;

	TurnPhase currentPhase = TurnPhase.TURN_STARTING;

	List<Player> players;
	public int playersTurn = -1;

	bool checkedPlayerStatus = true;


	// Use this for initialization
	void Start () {
		AddPlayers ();
		map = GetComponentInChildren<TileMap> ();
		map.Initialise ();
		unitManager = GetComponent<UnitManager> ();
		unitManager.Initialise (players, map);
		aiManager = GetComponent<AIManager> ();
		aiManager.Initialise (map);
		cameraManager = GetComponent<CameraManager> ();
		cameraManager.Initialise ();
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
		if (!checkedPlayerStatus) {
			if (currentPhase == TurnPhase.WAITING_FOR_INPUT && unitManager.PlayerOutOfActions (playersTurn)) {
				EndTurn ();
			} else {
				checkedPlayerStatus = true;
			}
		}
	}

	void AddPlayers() {
		players = new List<Player> ();

		Player p0 = new Player ();
		p0.id = 0;
		p0.name = "Player 1";
		p0.ai = false;
		p0.faction = 1;
		players.Add (p0);

		Player p1 = new Player ();
		p1.id = 1;
		p1.name = "Player 2";
		p1.ai = false;
		p1.faction = 2;
		players.Add (p1);

	}

	//FINITE STATE MACHINE

	public void StartNewTurn() {
		ChangeState(TurnPhase.TURN_STARTING);
		playersTurn++;
		playersTurn = playersTurn % players.Count;
		GetComponent<UnitManager> ().StartTurn (playersTurn);
		bool alliedTurn = !isAiTurn();

		GetComponentInChildren<UserInterfaceController> ().StartNewTurn (alliedTurn);

		if (players [playersTurn].ai) {
			aiManager.NewTurn (playersTurn);
		}
	}

	public void ChangeState(TurnPhase newPhase) {
		currentPhase = newPhase;
		checkedPlayerStatus = false;
	}

	public void FinishStartingTurn() {
		ChangeState(TurnPhase.WAITING_FOR_INPUT);
	}

	public void StartMoving() {
		ChangeState(TurnPhase.UNIT_MOVING);
	}

	public void FinishedMoving() {
		ChangeState(TurnPhase.WAITING_FOR_INPUT);
	}

	public void StartAttacking() {
		ChangeState(TurnPhase.UNIT_ATTACKING);
	}

	public void FinishedAttacking() {
		ChangeState(TurnPhase.WAITING_FOR_INPUT);
	}

	public void EndTurn() {
		ChangeState(TurnPhase.TURN_ENDING);
		//TODO CLEAN UP, EG SELECTED TILES
		unitManager.DeselectUnit();
		StartNewTurn ();
	}

	public bool isAiTurn() {
		return players [playersTurn].ai;
	}

	//MOVEMENT PHASE

	public void TileClicked(Node node, SquareTarget target) {
		
		if (currentPhase == TurnPhase.WAITING_FOR_INPUT && !isAiTurn()) {
			
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
		unitManager.MoveToTile (node);
		unitManager.DeselectUnit ();
	}

	public void ClickedUnselected(Node node) {
		if (node.myUnit != null && !unitManager.UnitAlreadySelected(node.myUnit)) {
			unitManager.DeselectUnit ();
			unitManager.SelectUnit (node.myUnit);

			if (node.myUnit.myPlayer.id == playersTurn) {
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
		}
	}

	public bool doesNodeContainPlayerUnit(Node node) {
		return node.myUnit != null && node.myUnit.myPlayer.id == playersTurn;
	}
}
