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

	UserInterfaceManager uIManager;
	GUIController gUIController;
	UnitManager unitManager;
	AIManager aiManager;
	CameraManager cameraManager;
	TileMap map;
	ObjectiveManager objectiveManager;

	TurnPhase currentPhase = TurnPhase.TURN_STARTING;

	List<Player> players;
	int playersTurn = -1;

	bool checkedPlayerStatus = true;

	// Use this for initialization
	public void Initialise () {
		AddPlayers ();
		uIManager = GetComponentInChildren<UserInterfaceManager> ();
		gUIController = GetComponentInChildren<GUIController> ();
		map = GetComponentInChildren<TileMap> ();
		map.Initialise ();
		unitManager = GetComponent<UnitManager> ();
		unitManager.Initialise (players, map);
		aiManager = GetComponent<AIManager> ();
		aiManager.Initialise (map);
		cameraManager = GetComponent<CameraManager> ();
		cameraManager.Initialise ();
		objectiveManager = GetComponent<ObjectiveManager> ();
		objectiveManager.Initialise ();
		AddObjectives ();
		StartNewTurn ();
	}
	
	// Update is called once per frame
	void Update () {
		//check to see if the turn should end
		if (!checkedPlayerStatus) {
			if (currentPhase == TurnPhase.WAITING_FOR_INPUT && unitManager.PlayerOutOfActions (playersTurn)) {
				EndTurn ();
			} else {
				checkedPlayerStatus = true;
			}
		}
	}

	//TEMP
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

	//TEMP
	void AddObjectives() {
		Objective objective = new Objective ();
		objective.optional = false;
		objective.text = "Kill all enemies!";
		objective.type = ObjectiveType.ANNIHILATION;
		objectiveManager.AddObjective (players[0], objective);

		Objective objective2 = new Objective ();
		objective2.optional = false;
		objective2.text = "Kill all enemies!";
		objective2.type = ObjectiveType.ANNIHILATION;
		objectiveManager.AddObjective (players[1], objective2);
	}

	public void StartNewTurn() {
		ChangeState(TurnPhase.TURN_STARTING);
		playersTurn++;
		playersTurn = playersTurn % players.Count;
		unitManager.StartTurn (playersTurn);
		bool alliedTurn = !isAiTurn();

		gUIController.StartNewTurn (alliedTurn, objectiveManager.getObjectives(players[playersTurn]));

		if (players [playersTurn].ai) {
			aiManager.NewTurn (playersTurn);
		}

		uIManager.StartTurn ();

		// Had error when unit died at turn start
		cameraManager.MoveToLocation (unitManager.SelectedUnit.myNode);
	}

	public void EndTurn() {
		ChangeState(TurnPhase.TURN_ENDING);
		unitManager.DeselectUnit();
		if (objectiveManager.CheckObjectives (players [playersTurn])) {
			Application.Quit ();
		} else {
			unitManager.EndTurn (playersTurn);
			StartNewTurn ();
			uIManager.EndTurn ();
		}
	}

	public TurnPhase CurrentPhase {
		get { return currentPhase; }
		set { ChangeState(value); }
	}

	public int PlayersTurn {
		get { return playersTurn; }
		set { playersTurn = value; }
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
		uIManager.FinishedMoving ();
	}

	public void StartAttacking() {
		ChangeState(TurnPhase.UNIT_ATTACKING);
	}

	public void FinishedAttacking() {
		ChangeState(TurnPhase.WAITING_FOR_INPUT);
		uIManager.FinishedAttacking ();
	}

	public bool isAiTurn() {
		return players [playersTurn].ai;
	}
}
