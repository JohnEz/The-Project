using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct DamageTarget {
	public UnitController target;
	public int damage;

	public DamageTarget(UnitController _target, int _damage) {
		target = _target;
		damage = _damage;
	}
}

public class UnitController : MonoBehaviour {
	
	[System.NonSerialized]
	public UnitStats myStats;
	[System.NonSerialized]
	public UnitManager myManager;
	UnitAnimationController anim;
	public Canvas unitCanvas;
	UnitAudioController audioController;

	[System.NonSerialized]
	public Vector2 facingDirection;

	//constants
	const float WALKSPEED = 3.25f;
	const float CLOSE_ENOUGH_TO_TILE = 0.005f;

	//Pathfinding
	List<Node> myPath = new List<Node>();
	[System.NonSerialized]
	public Node myNode;

	//Gameplay variables
	public Player myPlayer;

	List<DamageTarget> damageTargets = new List<DamageTarget>();

	// Use this for initialization
	void Start () {

	}

	public void Initialise() {
		anim = GetComponentInChildren<UnitAnimationController> ();
		myStats = GetComponent<UnitStats> ();
		myStats.Initailise ();
		audioController = GetComponent<UnitAudioController> ();
	}
	
	// Update is called once per frame
	void Update () {
		FollowPath ();
	}

	public void NewTurn() {
		myStats.HasMoved = false;
		myStats.ActionPoints = myStats.MaxActionPoints;
	}

	public void Spawn(Player player, Node startNode) {
		myPlayer = player;
		myNode = startNode;
	}

	public void DestroySelf() {
		Destroy (gameObject);
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

	public void SetSelected(bool selected) {
		anim.IsSelected (selected);
    }

	public void FinishedAttacking() {
		myManager.UnitFinishedAttacking ();
		HitDamageTargets ();
		ClearDamageTargets ();
		myStats.ActionPoints--;
	}

	public void RemoveTurn() {
		myStats.HasMoved = true;
		myStats.ActionPoints = 0;
	}

	public void FollowPath() {
		if (myPath.Count <= 0) {
			return;
		}

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

	public void SetPath(MovementPath newPath) {
		myPath = newPath.path;
		FaceDirection (myPath [0].previous.direction);
		SetWalking(true);
	}

	public void AddDamageTarget(UnitController target, int damage) {
		damageTargets.Add (new DamageTarget (target, damage));
	}

	public void HitDamageTargets() {
		foreach (DamageTarget damageTarget in damageTargets) {
			DealDamageTo (damageTarget.target, damageTarget.damage);
		}
	}

	public void ClearDamageTargets() {
		damageTargets.Clear ();
	}

	public bool TakeDamage(UnitController attacker, int damage) {
		//if the damage has a source
		if (attacker) {

		}

		int modifiedDamage = damage;

		myStats.SetHealth(myStats.Health - modifiedDamage);

		unitCanvas.GetComponent<UnitCanvasController> ().UpdateHP (myStats.Health, myStats.MaxHealth);
		unitCanvas.GetComponent<UnitCanvasController> ().CreateDamageText (modifiedDamage);

		if (myStats.Health > 0) {
			anim.PlayHitAnimation ();
		} else {
			anim.PlayDeathAnimation ();
			myManager.UnitDied (this);
		}

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

	public void PlayOneShot(AudioClip sound) {
		audioController.PlayOneShot (sound);
	}
}
