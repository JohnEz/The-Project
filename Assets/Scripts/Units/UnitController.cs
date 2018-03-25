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

	public GameObject unitCanvasPrefab;
	[System.NonSerialized]
	public UnitStats myStats;
	[System.NonSerialized]
	public UnitManager myManager;
	UnitAnimationController anim;
	UnitCanvasController unitCanvasController;
	UnitAudioController audioController;
	UnitClass myClass;

	[System.NonSerialized]
	public Vector2 facingDirection;


	//constants
	const float WALKSPEED = 325f;
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
	int effectsToCreate = 0;
	bool isAttackAnimationPlaying = false;
	List<GameObject> abilityEffects = new List<GameObject>();

	// Use this for initialization
	void Start () {

	}

	public void Initialise() {
		GameObject unitCanvas = Instantiate (unitCanvasPrefab);
		unitCanvas.transform.SetParent (transform, false);
		unitCanvasController = unitCanvas.GetComponent<UnitCanvasController> ();

		anim = GetComponentInChildren<UnitAnimationController> ();
		myStats = GetComponent<UnitStats> ();
		myStats.Initialise ();
		audioController = GetComponent<UnitAudioController> ();
		myClass = GetComponent<UnitClass> ();
		myClass.Initialise (this);
		projectiles = new List<ProjectileController> ();
	}
	
	// Update is called once per frame
	void Update () {
		FollowPath ();
	}

	public bool HasRemainingQueuedActions() {
		return actionQueue.Count > 0;
	}

	public void NewTurn() {
		//TODO i dont like the fact that unitstats cant set action points because of ui
		ActionPoints = myStats.MaxActionPoints;
		myStats.ApplyStartingTurnBuffs (
			(damage) => {this.TakeDamage(null, damage, true);}, 
			(healing) => {this.TakeHealing(null, healing);}
		);
		myStats.NewTurn ();
		myClass.NewTurn ();
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
			unitCanvasController.SetActionPoints (value);
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
		myPath[myPath.Count-1].myUnit = this;
		myNode = myPath [myPath.Count-1];
		FaceDirection (myPath [0].previous.direction);
		SetWalking(true);
		myManager.UnitStartedMoving ();
	}

	private void FinishWalking() {
		anim.IsWalking (false);
		RunNextAction (true);
		myManager.UnitFinishedMoving ();
	}

	public void AttackTarget(List<Node> targetNodes, BaseAbility ability) {
		currentAbilityTarget = targetNodes [0];
		if (ability.areaOfEffect == AreaOfEffect.SINGLE) {
			ability.UseAbility (currentAbilityTarget);
			FaceDirection (GetDirectionToTile(targetNodes[0]));
		} else {
			ability.UseAbility (targetNodes, currentAbilityTarget);
		}
		activeAbility = ability;
		SetAttacking (true);
		ActionPoints--;
		myManager.UnitStartedAttacking ();
		StartCoroutine (AttackRoutine());
	}

	public void SetAttackAnimationPlaying(bool isPlaying) {
		isAttackAnimationPlaying = isPlaying;
	}

	public IEnumerator AttackRoutine() {
		//make sure projects have been destroyed
		yield return new WaitUntil(() => !isAttackAnimationPlaying && projectiles.Count < 1);
		RunAbilityTargets ();

		//wait for effects to end
		yield return new WaitUntil(() => effectsToCreate == 0 && abilityEffects.Count < 1);

		ClearAbilityTargets ();
		currentAbilityTarget = null;
		myManager.UnitFinishedAttacking ();
		RunNextAction (true);
	}

	public void AddAbilityTarget(UnitController target, System.Action ability) {
		abilityTargets.Add (new AbilityTarget (target, ability));
	}

	//TODO COME UP WITH A BETTER NAME - means deal damage / show cast events
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
		//effectsToCreate = 0;
		//abilityEffects.Clear ();
	}

	public bool TakeDamage(UnitController attacker, int damage, bool ignoreArmour = false, bool crit = false) {
		bool isStillAlive = true;

		//if the damage has a source
		if (attacker) {
			//this could be used if the character as a reposte etc
		}

		int modifiedDamage = ignoreArmour ? damage : Mathf.Max(damage - myStats.DamageReduction, 0);

		//check to see if attack was blocked
		float blockRoll = Random.value * 100;
		bool blocked = false;
		if (!ignoreArmour && !crit && blockRoll <= myStats.Block) {
			blocked = true;
			modifiedDamage = (int)(modifiedDamage * 0.5f);
			unitCanvasController.CreateBasicText ("Block");
		}

		myStats.SetHealth(myStats.Health - modifiedDamage);

		unitCanvasController.UpdateHP (myStats.Health, myStats.MaxHealth);
		unitCanvasController.CreateDamageText (modifiedDamage.ToString ());

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

	public bool DealDamageTo(UnitController target, float damage, bool ignoreArmour = false) {

		float endDamage = myStats.Power * damage;
		bool crit = false;

		//check to see if damage is a crit
		float critRoll = Random.value * 100;
		if (critRoll <= myStats.Crit) {
			crit = true;
			endDamage = endDamage * 1.5f;
		}
			
		return target.TakeDamage (this, (int)endDamage, ignoreArmour, crit);
	}

	public bool TakeHealing(UnitController caster, int healing) {

		myStats.SetHealth(myStats.Health + healing);

		unitCanvasController.UpdateHP (myStats.Health, myStats.MaxHealth);
		unitCanvasController.CreateHealText (healing.ToString ());

		return true;
	}

	public bool GiveHealingTo(UnitController target, float healing) {

		float endHealing = myStats.Power * healing;

		//check to see if damage is a crit
		float critRoll = Random.value * 100;
		if (critRoll <= myStats.Crit) {
			endHealing = endHealing * 1.5f;
		}

		return target.TakeHealing (this, (int)endHealing);
	}

	public void ApplyBuff(Buff buff) {
		bool buffAdded = myStats.ApplyBuff (buff);
		if (buffAdded && buff.persistentFxPrefab) {
			CreateBuffEffect (buff);
		}
	}

	public void Dispell(bool debuff) {
		if (myStats.Buffs.Count > 0) {
			Buff buffToDispell = myStats.FindNewestBuff (debuff);
			if (buffToDispell != null) {
				myStats.RemoveBuff (buffToDispell, true);
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

	IEnumerator CreateEffect(GameObject effect, float delay = 0) {
		return CreateEffectAtLocation (myNode, effect, delay);
	}

	IEnumerator CreateEffectAtLocation(Node location, GameObject effect, float delay = 0) {
		yield return new WaitForSeconds (delay);
		GameObject myEffect =  Instantiate (effect);
		myEffect.transform.SetParent (location.transform, false);
		myEffect.GetComponent<SpriteFxController> ().Initialise (this);
		abilityEffects.Add (myEffect);
		effectsToCreate--;
	}

	public void CreateEffectWithDelay(GameObject effect, float delay, Node location = null) {
		effectsToCreate++;
		if (location != null) {
			StartCoroutine (CreateEffectAtLocation (location, effect, delay));
		} else {
			StartCoroutine (CreateEffect (effect, delay));
		}
	}

	public void RemoveEffect(GameObject effectToRemove) {
		abilityEffects.Remove (effectToRemove);
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
