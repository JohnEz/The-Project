using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct AbilityTarget {
	public UnitController target;
	public System.Action abilityFunction;

	public AbilityTarget(UnitController _target, System.Action _ability) {
		target = _target;
		abilityFunction = _ability;
	}
}

public enum ActionType {
	MOVEMENT,
	ATTACK,
	NULL
}

public struct Action {
	public ActionType type;
	public List<Node> nodes;
	public BaseAbility ability;
}

public class UnitController : MonoBehaviour {
	
	[System.NonSerialized]
	public UnitStats myStats;
	[System.NonSerialized]
	public UnitManager myManager;
	UnitAnimationController anim;
	public Canvas unitCanvas;
	UnitAudioController audioController;
	UnitClass myClass;

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

	//Ability variables
	BaseAbility activeAbility;
	List<AbilityTarget> abilityTargets = new List<AbilityTarget>();
	Queue<Action> actionQueue = new Queue<Action>();
	List<ProjectileController> projectiles;
	Node currentAbilityTarget;

	// Use this for initialization
	void Start () {

	}

	public void Initialise() {
		anim = GetComponentInChildren<UnitAnimationController> ();
		myStats = GetComponent<UnitStats> ();
		myStats.Initailise ();
		audioController = GetComponent<UnitAudioController> ();
		myClass = GetComponent<UnitClass> ();
		projectiles = new List<ProjectileController> ();
	}
	
	// Update is called once per frame
	void Update () {
		FollowPath ();
	}

	public void NewTurn() {
		//TODO i dont like the fact that unitstats cant set action points because of ui
		ActionPoints = myStats.MaxActionPoints;
		myStats.NewTurn ();
	}

	public void EndTurn() {
		ActionPoints = 0;
		myStats.EndTurn ();
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
		
	Vector2 GetDirectionToTile(Node target) {
		//find if we want to check x or y
		float difX = target.x - myNode.x;
		float difY = target.y - myNode.y;

		//This needs to be changed if we get 4 directions to x >= y
		if (Mathf.Abs (difX) > 0) {
			return new Vector2 (Mathf.Sign (difX), 0);
		} else {
			return new Vector2 (0, Mathf.Sign(difY));
		}
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

	public void RemoveTurn() {
		ActionPoints = 0;
	}

	public int ActionPoints {
		get { return myStats.ActionPoints; }
		set { 
			myStats.ActionPoints = value;
			unitCanvas.GetComponent<UnitCanvasController> ().SetActionPoints (value);
		}
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
				FinishWalking ();
			}
			myPath.RemoveAt (0);
		}
	}

	public bool AddAction(Action action) {
		actionQueue.Enqueue (action);

		if (actionQueue.Count == 1) {
			RunNextAction (false);
		}

		return true;
	}

	private void RunNextAction(bool removeAction) {

		if (removeAction) {
			actionQueue.Dequeue ();
		}

		if (actionQueue.Count >= 1) {
			Action nextAction = actionQueue.Peek ();

			switch (nextAction.type) {
			case ActionType.MOVEMENT:
				SetPath (nextAction.nodes);
				break;
			case ActionType.ATTACK:
				AttackTarget (nextAction.nodes, nextAction.ability);
				break;
			}
		}
	}

	public void SetPath(List<Node> path) {
		if (path [path.Count - 1].cost > myStats.Speed) {
			ActionPoints -= 2;
		} else {
			ActionPoints--;
		}
		myNode.myUnit = null;
		myPath = path;
		FaceDirection (myPath [0].previous.direction);
		SetWalking(true);
		myManager.UnitStartedMoving ();
	}

	private void FinishWalking() {
		anim.IsWalking (false);
		myPath [0].myUnit = this;
		myNode = myPath [0];
		myManager.UnitFinishedMoving ();
		RunNextAction (true);
	}

	public void AttackTarget(List<Node> targetNodes, BaseAbility ability) {
		currentAbilityTarget = targetNodes [0];
		if (ability.areaOfEffect == AreaOfEffect.SINGLE) {
			ability.UseAbility (this, currentAbilityTarget);
			FaceDirection (GetDirectionToTile(targetNodes[0]));
		} else {
			ability.UseAbility (this, targetNodes, currentAbilityTarget);
		}
		activeAbility = ability;
		SetAttacking (true);
		ActionPoints--;
		myManager.UnitStartedAttacking ();
	}

	public void FinishedAttacking() {
		myManager.UnitFinishedAttacking ();
		RunAbilityTargets ();
		ClearAbilityTargets ();
		currentAbilityTarget = null;
		RunNextAction (true);
	}

	public void AddAbilityTarget(UnitController target, System.Action ability) {
		abilityTargets.Add (new AbilityTarget (target, ability));
	}

	public void RunAbilityTargets() {
		foreach (AbilityTarget target in abilityTargets) {
			target.abilityFunction ();
			activeAbility.eventActions.ForEach ((eventAction) => {
				if (eventAction.eventTrigger == Event.CAST_END && eventAction.eventTarget == EventTarget.TARGETUNIT) {
					eventAction.action(this, target.target, target.target.myNode);
				}
			});
		}

		activeAbility.eventActions.ForEach ((eventAction) => {
			if (eventAction.eventTrigger == Event.CAST_END) {
				if (eventAction.eventTarget == EventTarget.CASTER || eventAction.eventTarget == EventTarget.TARGETEDTILE) {
					eventAction.action(this, null, currentAbilityTarget);
				}
			}
		});
	}

	public void ClearAbilityTargets() {
		abilityTargets.Clear ();
	}

	public bool TakeDamage(UnitController attacker, int damage) {
		bool isStillAlive = true;

		//if the damage has a source
		if (attacker) {

		}

		int modifiedDamage = damage - myStats.Armour;

		//check to see if attack was blocked
		float blockRoll = Random.value * 100;
		if (blockRoll <= myStats.Block) {
			modifiedDamage = (int)(modifiedDamage * 0.5f);
		}

		myStats.SetHealth(myStats.Health - modifiedDamage);

		unitCanvas.GetComponent<UnitCanvasController> ().UpdateHP (myStats.Health, myStats.MaxHealth);
		unitCanvas.GetComponent<UnitCanvasController> ().CreateDamageText (modifiedDamage.ToString ());

		if (myStats.Health > 0) {
			anim.PlayHitAnimation ();
			if (myClass.onHitSfx) {
				PlayOneShot (myClass.onHitSfx);
			}
		} else {
			anim.PlayDeathAnimation ();
			if (myClass.onDeathSfx) {
				PlayOneShot (myClass.onDeathSfx);
			}
			myManager.UnitDied (this);
			isStillAlive = false;
		}
			
		return isStillAlive;
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

	public bool TakeHealing(UnitController caster, int healing) {

		myStats.SetHealth(myStats.Health + healing);

		unitCanvas.GetComponent<UnitCanvasController> ().UpdateHP (myStats.Health, myStats.MaxHealth);
		unitCanvas.GetComponent<UnitCanvasController> ().CreateHealText (healing.ToString ());

		return true;
	}

	public bool GiveHealingTo(UnitController target, int healing) {

		int endHealing = healing;

		//add power to attack
		endHealing += myStats.Power;

		//check to see if damage is a crit
		float critRoll = Random.value * 100;
		if (critRoll <= myStats.Crit) {
			endHealing = (int)(endHealing * 1.5f);
		}

		return target.TakeHealing (this, endHealing);
	}

	public void ApplyBuff(Buff buff) {
		myStats.Buffs.Add (buff);
		if (buff.persistentFxPrefab) {
			CreateBuffEffect (buff);
		}
	}

	public void Dispell(bool debuff) {
		if (myStats.Buffs.Count > 0) {
			Buff buffToDispell = myStats.FindFirstBuff (debuff);
			if (buffToDispell != null) {
				myStats.RemoveBuff (buffToDispell);
			}
		}
	}

	public void PlayOneShot(AudioClip sound) {
		audioController.PlayOneShot (sound);
	}

	public void CreateBuffEffect(Buff buff) {
		GameObject myEffect =  Instantiate (buff.persistentFxPrefab);
		myEffect.transform.SetParent (transform, false);
		buff.persistentFx = myEffect;
	}

	public IEnumerator CreateEffect(GameObject effect, float delay = 0) {
		yield return new WaitForSeconds (delay);
		GameObject myEffect =  Instantiate (effect);
		myEffect.transform.SetParent (transform, false);
	}

	static IEnumerator CreateEffectAtLocation(Node location, GameObject effect, float delay = 0) {
		yield return new WaitForSeconds (delay);
		GameObject myEffect =  Instantiate (effect);
		myEffect.transform.SetParent (location.transform, false);
	}

	public void CreateEffectWithDelay(GameObject effect, float delay, Node location = null) {
		if (location != null) {
			StartCoroutine (CreateEffectAtLocation (location, effect, delay));
		} else {
			StartCoroutine (CreateEffect (effect, delay));
		}
	}

	public IEnumerator CreateProjectile(GameObject projectile, Node target, float speed, float delay = 0) {
		yield return new WaitForSeconds (delay);
		GameObject createdProjectile = Instantiate (projectile);

		ProjectileController createdProjectileController = createdProjectile.GetComponent<ProjectileController> ();
		createdProjectileController.SetTarget (this, target, speed);
		projectiles.Add (createdProjectileController);
	}

	public void CreateProjectileWithDelay(GameObject projectile, Node target, float speed, float delay) {
		StartCoroutine(CreateProjectile (projectile, target, speed, delay));
	}

	public void ProjectileHit(ProjectileController projectile) {
		projectiles.Remove (projectile);
		Destroy (projectile.gameObject);
	}
}
