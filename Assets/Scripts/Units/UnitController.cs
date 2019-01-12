﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AbilityTarget {
    public Node targetNode;
    public System.Action abilityFunction;

    public AbilityTarget(Node _targetNode, System.Action _ability) {
        targetNode = _targetNode;
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
    public AttackAction ability;
}

public class UnitController : MonoBehaviour {
    public GameObject unitCanvasPrefab;
    public UnitObject baseStats;

    [System.NonSerialized]
    public UnitObject myStats;

    [System.NonSerialized]
    public UnitManager myManager;

    private UnitCanvasController unitCanvasController;
    private UnitDialogController myDialogController;

    [System.NonSerialized]
    public Vector2 facingDirection;

    //constants
    private const float WALKSPEED = 325f;

    private const float CLOSE_ENOUGH_TO_TILE = 0.005f;

    //Pathfinding
    private List<Node> myPath = new List<Node>();

    [System.NonSerialized]
    public Node myNode;

    //Gameplay variables
    public Player myPlayer;

    //Ability variables
    private AttackAction activeAction;

    private List<AbilityTarget> abilityTargets = new List<AbilityTarget>();
    private Queue<Action> actionQueue = new Queue<Action>();
    private List<ProjectileController> projectiles;
    private Node currentAbilityTarget;
    private int effectsToCreate = 0;
    private List<GameObject> abilityEffects = new List<GameObject>();

    // Use this for initialization
    private void Start() {
    }

    public void Initialise() {
        GameObject unitCanvas = Instantiate(unitCanvasPrefab);
        unitCanvas.transform.SetParent(transform, false);
        unitCanvasController = unitCanvas.GetComponent<UnitCanvasController>();

        myStats = Instantiate(baseStats);
        myStats.Initialise(this);
        projectiles = new List<ProjectileController>();
        myDialogController = GetComponentInChildren<UnitDialogController>();
    }

    // Update is called once per frame
    private void Update() {
        if (abilityEffects.Count > 0  || effectsToCreate > 0) {
            Debug.Log("effectsToCreate: " + effectsToCreate);
            Debug.Log("abilityEffects: " + abilityEffects.Count);
        }
        FollowPath();
    }

    public bool HasRemainingQueuedActions() {
        return actionQueue.Count > 0;
    }

    public void NewTurn() {
        myStats.ApplyStartingTurnBuffs(
            (damage) => { this.TakeDamage(null, damage, true); },
            (healing) => { this.TakeHealing(null, healing); }
        );
        myStats.NewTurn();
        Stamina = myStats.MaxStamina;
    }

    public void EndTurn() {
        myStats.EndTurn();
    }

    public void Spawn(Player player, Node startNode) {
        myPlayer = player;
        myNode = startNode;
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }

    public void FaceDirection(Vector2 dir) {
        //GetComponentInChildren<UnitAnimationController>().FaceDirection(dir);
        facingDirection = dir;
    }

    private Vector2 GetDirectionToTile(Node target) {
        //find if we want to check x or y
        float difX = target.x - myNode.x;
        float difY = target.y - myNode.y;

        //This needs to be changed if we get 4 directions to x >= y
        if (Mathf.Abs(difX) > 0) {
            return new Vector2(Mathf.Sign(difX), 0);
        } else {
            return new Vector2(0, Mathf.Sign(difY));
        }
    }

    public void SetWalking(bool walking) {
        //anim.IsWalking(walking);
    }

    public void SetAttacking(bool attacking) {
        //anim.IsAttacking(attacking);
    }

    public void SetSelected(bool selected) {
        //anim.IsSelected(selected);
    }

    public void RemoveTurn() {
    }

    public int Stamina {
        get { return myStats.Stamina; }
        set {
            myStats.SetStamina(value);
            unitCanvasController.UpdateStamina(myStats.Stamina, myStats.MaxStamina);
            GUIController.singleton.UpdateStamina(myStats.Stamina);
        }
    }

    public int Health {
        get { return myStats.Health; }
        set {
            myStats.SetHealth(value);
            unitCanvasController.UpdateHP(myStats.Health, myStats.MaxHealth);
        }
    }

    public void FollowPath() {
        if (myPath.Count <= 0) {
            return;
        }

        float distanceToNode = Vector3.Distance(myPath[0].transform.position, transform.position);

        if (distanceToNode - (WALKSPEED * Time.deltaTime) > CLOSE_ENOUGH_TO_TILE) {
            transform.position = transform.position + ((Vector3)facingDirection * WALKSPEED * Time.deltaTime);
        } else {
            transform.position = myPath[0].transform.position;
            if (myPath.Count > 1) {
                FaceDirection(myPath[1].previous.GetDirectionFrom(myPath[1]));
            } else {
                FinishWalking();
            }
            myPath.RemoveAt(0);
        }
    }

    public bool AddAction(Action action) {
        actionQueue.Enqueue(action);

        // Why did i do this?
        if (actionQueue.Count == 1) {
            RunNextAction(false);
        }

        return true;
    }

    private void RunNextAction(bool removeAction) {
        if (removeAction && actionQueue.Count > 0) {
            actionQueue.Dequeue();
        }

        if (actionQueue.Count >= 1) {
            Action nextAction = actionQueue.Peek();

            switch (nextAction.type) {
                case ActionType.MOVEMENT:
                    SetPath(nextAction.nodes);
                    break;

                case ActionType.ATTACK:
                    AttackTarget(nextAction.nodes, nextAction.ability);
                    break;
            }
        }
    }

    public void SetPath(List<Node> path) {
        myNode.myUnit = null;
        myPath = path;
        myPath[myPath.Count - 1].myUnit = this;
        myNode = myPath[myPath.Count - 1];
        FaceDirection(myPath[0].previous.GetDirectionFrom(myPath[0]));
        SetWalking(true);
        myManager.UnitStartedMoving();
    }

    private void FinishWalking() {
        //anim.IsWalking(false);

        // TODO only the player should be able to open doors
        if (myNode.HasDoor()) {
            myNode.OpenDoors();
        }

        RunNextAction(true);
        myManager.UnitFinishedMoving(this);
    }

    public void AttackTarget(List<Node> targetNodes, AttackAction action) {
        currentAbilityTarget = targetNodes[0];
        if (action.areaOfEffect == AreaOfEffect.SINGLE) {
            action.UseAbility(currentAbilityTarget);
            FaceDirection(GetDirectionToTile(targetNodes[0]));
        } else {
            action.UseAbility(targetNodes, currentAbilityTarget);
        }
        myManager.UnitStartedAttacking();
        activeAction = action;
        SetAttacking(true);
        myDialogController.Attacking();
        effectsToCreate = action.eventActions.FindAll(e => e.GetType() == typeof(VisualEffectEventAction)).Count;
        Debug.Log("setup effectsToCreate: " + effectsToCreate);
        Debug.Log("eventActions: " + action.eventActions.Count);
        StartCoroutine(AttackRoutine());
    }

    public bool getAttackAnimationPlaying() {
        return false;
    }

    public bool getAttackHasLanded() {
        return true;
    }

    public IEnumerator AttackRoutine() {
        //make sure projects have been destroyed
        yield return new WaitUntil(() => getAttackHasLanded() && projectiles.Count < 1);
        RunAbilityTargets();

        //wait for effects to end
        yield return new WaitUntil(() => !getAttackAnimationPlaying() && effectsToCreate == 0 && abilityEffects.Count < 1);

        ClearAbilityTargets();
        currentAbilityTarget = null;
        myManager.UnitFinishedAttacking();
        RunNextAction(true);
    }

    public void AddAbilityTarget(Node targetNode, System.Action ability) {
        abilityTargets.Add(new AbilityTarget(targetNode, ability));
    }

    //TODO COME UP WITH A BETTER NAME - means deal damage / show cast events
    public void RunAbilityTargets() {
        // for each target unit, run effects and damage
        Debug.Log("abilityTargets: " + abilityTargets.Count);
        foreach (AbilityTarget abilityTarget in abilityTargets) {
            abilityTarget.abilityFunction();
            activeAction.eventActions.ForEach((eventAction) => {
                if (eventAction.eventTrigger == AbilityEvent.CAST_END && eventAction.eventTarget == EventTarget.TARGETUNIT) {
                    Debug.Log("Creating effect from ability targets: " + eventAction.eventTrigger + ", " + eventAction.eventTarget);
                    eventAction.action(this, abilityTarget.targetNode.myUnit, abilityTarget.targetNode);
                }
            });
        }

        // for each effect for node and caster, run now
        activeAction.eventActions.ForEach((eventAction) => {
            if (eventAction.eventTrigger == AbilityEvent.CAST_END) {
                if (eventAction.eventTarget == EventTarget.CASTER || eventAction.eventTarget == EventTarget.TARGETEDTILE) {
                    eventAction.action(this, null, currentAbilityTarget);
                }
            }
        });
    }

    public void ClearAbilityTargets() {
        abilityTargets.Clear();
        effectsToCreate = 0;
        abilityEffects.Clear ();
    }

    public bool TakeDamage(UnitController attacker, int damage, bool ignoreArmour = false) {
        bool isStillAlive = true;

        //if the damage has a source
        if (attacker) {
            //this could be used if the character as a reposte etc
        }

        int modifiedDamage = ignoreArmour ? damage : Mathf.Max(damage - myStats.Armour, 0);

        //check to see if attack was blocked
        float blockRoll = Random.value * 100;
        if (!ignoreArmour && blockRoll <= myStats.Block) {
            modifiedDamage = (int)(modifiedDamage * 0);
            unitCanvasController.CreateBasicText("Block");
        }

        Health -= modifiedDamage;
        unitCanvasController.CreateDamageText(modifiedDamage.ToString());

        myDialogController.Attacked();

        if (myStats.Health > 0) {
            //anim.PlayHitAnimation();
            // TODO move these onto the unit stats
            if (myStats.onHitSfx) {
                AudioManager.singleton.Play(myStats.onHitSfx, transform, AudioMixers.SFX);
            }
        } else {
            //anim.PlayDeathAnimation();
            if (myStats.onDeathSfx) {
                AudioManager.singleton.Play(myStats.onDeathSfx, transform, AudioMixers.SFX);
            }
            myManager.UnitDied(this);
            isStillAlive = false;
        }

        return isStillAlive;
    }

    public bool DealDamageTo(UnitController target, int damage, bool ignoreArmour = false) {
        return target.TakeDamage(this, damage, ignoreArmour);
    }

    public bool TakeHealing(UnitController caster, int healing) {
        Health += healing;
        unitCanvasController.CreateHealText(healing.ToString());

        myDialogController.Helped();

        return true;
    }

    public bool GiveHealingTo(UnitController target, float healing) {
        float endHealing = healing;

        myDialogController.Helping();

        return target.TakeHealing(this, (int)endHealing);
    }

    public void ApplyBuff(Buff buff) {
        bool buffAdded = myStats.ApplyBuff(buff);
        if (buffAdded && buff.persistentFxPrefab) {
            CreateBuffEffect(buff);
        }
    }

    public void Dispell(bool debuff) {
        if (myStats.Buffs.Count > 0) {
            Buff buffToDispell = myStats.FindNewestBuff(debuff);
            if (buffToDispell != null) {
                myStats.RemoveBuff(buffToDispell, true);
            }
        }
    }

    public void Summon(Node targetNode, GameObject unitPrefab) {
        // TODO work out a clean way of getting allied player
        Player owningPlayer = myPlayer.ai ? myPlayer : PlayerManager.singleton.GetPlayer(1);
        UnitManager.singleton.SpawnUnit(unitPrefab, owningPlayer, targetNode.x, targetNode.y);
    }

    public void CreateBuffEffect(Buff buff) {
        GameObject myEffect = Instantiate(buff.persistentFxPrefab);
        myEffect.transform.SetParent(transform, false);
        buff.persistentFx = myEffect;
    }

    private IEnumerator CreateEffect(GameObject effect, float delay = 0) {
        return CreateEffectAtLocation(myNode, effect, delay);
    }

    private IEnumerator CreateEffectAtLocation(Node location, GameObject effect, float delay = 0) {
        yield return new WaitForSeconds(delay);
        GameObject myEffect = Instantiate(effect);
        myEffect.transform.SetParent(location.transform, false);
        myEffect.GetComponent<SpriteFxController>().Initialise(this);
        abilityEffects.Add(myEffect);
        Debug.Log("Created Effect");
        effectsToCreate--;
    }

    public void CreateEffectWithDelay(GameObject effect, float delay, Node location = null) {
        if (location != null) {
            StartCoroutine(CreateEffectAtLocation(location, effect, delay));
        } else {
            StartCoroutine(CreateEffect(effect, delay));
        }
    }

    public void RemoveEffect(GameObject effectToRemove) {
        abilityEffects.Remove(effectToRemove);
    }

    public IEnumerator CreateProjectile(GameObject projectile, Node target, float speed, float delay = 0) {
        yield return new WaitForSeconds(delay);
        GameObject createdProjectile = Instantiate(projectile);

        ProjectileController createdProjectileController = createdProjectile.GetComponent<ProjectileController>();
        createdProjectileController.SetTarget(this, target, speed);
        projectiles.Add(createdProjectileController);
    }

    public void CreateProjectileWithDelay(GameObject projectile, Node target, float speed, float delay) {
        StartCoroutine(CreateProjectile(projectile, target, speed, delay));
    }

    public void ProjectileHit(ProjectileController projectile) {
        projectiles.Remove(projectile);
        Destroy(projectile.gameObject);
    }
}