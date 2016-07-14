using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitController : MonoBehaviour {

	public Node myNode;

	UnitAnimationController anim;
	public UnitManager myManager;

	public Vector2 facingDirection;



	//constants
	const float WALKSPEED = 2.75f;
	const float CLOSE_ENOUGH_TO_TILE = 0.005f;

	//Pathfinding
	List<Node> myPath;

	//Gameplay variables
	public int myTeam = 1;
	public int myPlayer = 1;
	public int actionPoints = 1;

	//Basic Stats
	public int movementSpeed = 3;
	public Walkable walkingType = Walkable.Walkable;

	// Use this for initialization
	void Start () {
		Initialise ();
	}

	public void Initialise() {
		anim = GetComponentInChildren<UnitAnimationController> ();
		myPath = new List<Node> ();
	}
	
	// Update is called once per frame
	void Update () {
		FollowPath ();
	}

	public void NewTurn() {
		actionPoints = 1;
	}

	public void Spawn(int team, int player, Node startNode) {
		myTeam = team;
		myPlayer = player;
		myNode = startNode;
	}

	public void FaceDirection(Vector2 dir) {
		GetComponentInChildren<UnitAnimationController> ().FaceDirection (dir);
		facingDirection = dir;
	}

	public void SetWalking(bool walking) {
		anim.isWalking (walking);
	}

	public void FollowPath() {
		if (myPath.Count > 0) {
			float distanceToNode = Vector3.Distance (myPath[0].transform.position, transform.position);

			if (distanceToNode - (WALKSPEED * Time.deltaTime) > CLOSE_ENOUGH_TO_TILE) {
				transform.position = transform.position + ((Vector3)facingDirection * WALKSPEED * Time.deltaTime);
			} else {
				transform.position = myPath [0].transform.position;
				if (myPath.Count > 1) {
					FaceDirection (myPath [1].previous.direction);
				} else {
					anim.isWalking (false);
					myPath [0].myUnit = this;
					myNode = myPath [0];
					myManager.UnitFinishedMoving ();
				}
				myPath.RemoveAt (0);
			}
		}
	}

	public void SetPath(MovementPath newPath) {
		myPath = newPath.path;
		//TODO REMOVE MOVEMENT COST
		FaceDirection (myPath [0].previous.direction);
		anim.isWalking (true);
	}
}
