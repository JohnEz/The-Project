using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitController : MonoBehaviour {
	
	[System.NonSerialized]
	public UnitStats myStats;
	[System.NonSerialized]
	public UnitManager myManager;
	UnitAnimationController anim;

	[System.NonSerialized]
	public Vector2 facingDirection;

	//constants
	const float WALKSPEED = 3.25f;
	const float CLOSE_ENOUGH_TO_TILE = 0.005f;

	//Pathfinding
	List<Node> myPath;
	[System.NonSerialized]
	public Node myNode;

	//Gameplay variables
	public int myTeam = 1;
	public int myPlayer = 1;

	// Use this for initialization
	void Start () {

	}

	public void Initialise() {
		anim = GetComponentInChildren<UnitAnimationController> ();
		myStats = GetComponent<UnitStats> ();
		myStats.Initailise ();
		myPath = new List<Node> ();
	}
	
	// Update is called once per frame
	void Update () {
		FollowPath ();
	}

	public void NewTurn() {
		myStats.ActionPoints = myStats.MaxActionPoints;
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
		anim.IsWalking (walking);
	}

	public void SetAttacking(bool attacking) {
		anim.IsAttacking (attacking);
	}

	public void FinishedAttacking() {
		myManager.UnitFinishedAttacking ();
		myStats.ActionPoints--;
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
					anim.IsWalking (false);
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
		anim.IsWalking (true);
	}

	public bool TakeDamage(UnitController attacker, int damage) {
		//if the damage has a source
		if (attacker) {

		}

		myStats.Health -= damage;
		return true;
	}

	public bool DealDamageTo(UnitController target, int damage) {

		int endDamage = damage;

		//add power to attack
		endDamage += myStats.Power;

		//check to see if damage is a crit
		float critRoll = Random.value * 100;
		if (critRoll <= myStats.Crit) {
			endDamage = (int)(endDamage * 1.5f);
		}
			
		return target.TakeDamage (this, endDamage);
	}
}
