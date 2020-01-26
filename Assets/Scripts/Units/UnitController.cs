using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    public List<Node> effectedNodes;
    public List<Tile> moveTiles;
    public AttackAction ability;
}

public struct EffectOptions {
    public GameObject effect;
    public float delay;
    public Node location;
    public bool rotateWithCharacter;

    public EffectOptions(GameObject _effect, float _delay) {
        effect = _effect;
        delay = _delay;
        location = null;
        rotateWithCharacter = false;
    }
}

public class UnitController : MonoBehaviour {
    private const float DAMAGE_LOWER_BOUND = 0.75f;
    private const float DAMAGE_UPPER_BOUND = 1.25f;
    private const float BLOCK_MODIFIER = 0.5f;
    private const float CRIT_MODIFIER = 2f;

    public GameObject unitCanvasPrefab;
    public UnitObject baseStats;

    public UnitStatistics myCounters;

    [System.NonSerialized]
    public UnitObject myStats;

    [System.NonSerialized]
    public UnitManager myManager;

    private UnitCanvasController unitCanvasController;
    private UnitDialogController myDialogController;

    [System.NonSerialized]
    public Vector2 facingDirection;

    //constants
    private const float WALKSPEED = 32.5f;

    private const float CLOSE_ENOUGH_TO_TILE = 0.005f;

    //Pathfinding
    private List<Tile> myPath = new List<Tile>();

    [System.NonSerialized]
    public Tile myTile;

    private Tile forceMovedTile;

    //Gameplay variables
    public Player myPlayer;

    //Ability variables
    private AttackAction activeAction;

    private List<AbilityTarget> abilityTargets = new List<AbilityTarget>();
    private Queue<Action> actionQueue = new Queue<Action>();
    private List<ProjectileController> projectiles;
    private Node currentAbilityTarget;
    private int projectilesToCreate = 0;
    private int effectsToCreate = 0;
    private List<GameObject> abilityEffects = new List<GameObject>();

    // Use this for initialization
    private void Start() {
    }

    public void Initialise() {
        GameObject unitCanvas = Instantiate(unitCanvasPrefab);
        unitCanvas.transform.SetParent(transform, false);
        unitCanvasController = unitCanvas.GetComponent<UnitCanvasController>();

        myCounters = new UnitStatistics();
        myStats.Initialise(this);
        projectiles = new List<ProjectileController>();
        myDialogController = GetComponentInChildren<UnitDialogController>();

        Activate();
    }

    // Update is called once per frame
    private void Update() {
        FollowPath();
        UpdateForceMovement();
    }

    public bool IsAllyOf(UnitController other) {
        return myPlayer.faction == other.myPlayer.faction;
    }

    public bool IsStealthed() {
        return myStats.buffs.FindBuff("Stealth") != null;
    }

    public bool IsStunned() {
        return myStats.buffs.FindBuff("Stun") != null;
    }

    public bool IsTaunted() {
        return myStats.buffs.FindBuff("Taunt") != null;
    }

    public List<UnitController> GetTaunters() {
        return myStats.buffs.FindBuffs("Taunt").Select((taunt) => ((Taunt)taunt).taunter).ToList();
    }

    public bool HasRemainingQueuedActions() {
        return actionQueue.Count > 0;
    }

    public void NewTurn() {
        if (IsStunned()) {
            unitCanvasController.CreateBasicText("Stunned");
        } else {
            ActionPoints = myStats.MaxActionPoints;
        }

        myStats.NewTurn();
        myCounters.NewTurn();
    }

    public void EndTurn() {
        myStats.EndTurn();
        myCounters.EndTurn();
        ActionPoints = 0;
    }

    public void Spawn(Player player, Tile startTile, UnitObject startingStats) {
        myPlayer = player;
        myTile = startTile;
        myStats = startingStats;
    }

    public void Activate() {
        gameObject.SetActive(true);

        // TODO sort this problem, it shouldnt try to create camera for allies and the player, only enemies
        if (PlayerManager.instance.mainPlayer == this.myPlayer) {
            return;
        }

        if (!SavedVariables.HasEncounteredEnemy(myStats.className)) {
            SavedVariables.EncounteredEnemy(myStats.className);

            CameraManager.instance.AddEncounteredTarget(this);
        }
    }

    public void DestroySelf() {
        Destroy(gameObject, 1f);
    }

    public void FaceDirection(Vector2 dir) {
        if (dir.magnitude == 0) {
            return;
        }

        float rotationX = 90 * -dir.x;
        float rotationY = dir.y == 1 ? 180 : 0;

        transform.Find("Token").transform.localRotation = Quaternion.Euler(new Vector3(0, rotationX + rotationY, 0));

        facingDirection = dir;
    }

    public Vector2 GetDirectionToTile(Node target) {
        if (myTile.Nodes.Contains(target)) {
            return new Vector2(0, 0);
        }

        Neighbour neighbour = myTile.FindNeighbourTo(target);

        if (neighbour != null) {
            return neighbour.GetDirectionFrom(target);
        }

        //find if we want to check x or y
        float minusX = target.x - myTile.x;
        float minusY = myTile.y - target.y;
        float difX = Mathf.Abs(minusX);
        float difY = Mathf.Abs(minusY);

        if (difX >= difY) {
            return new Vector2(Mathf.Sign(minusX), 0);
        } else {
            return new Vector2(0, Mathf.Sign(minusY));
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

    public int ActionPoints {
        get { return myStats.ActionPoints; }
        set {
            myStats.ActionPoints = value;
            if (unitCanvasController != null) {
                unitCanvasController.UpdateActionPoints(value);
            }
        }
    }

    public bool IsBeingForceMoved() {
        return forceMovedTile != null;
    }

    public void FollowPath() {
        if (myPath.Count <= 0) {
            return;
        }
        Vector3 my2DPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 target2DPosition = new Vector3(myPath[0].transform.position.x, 0, myPath[0].transform.position.z);
        float distanceToNode = Vector3.Distance(my2DPosition, target2DPosition);

        if (distanceToNode - (WALKSPEED * Time.deltaTime) > CLOSE_ENOUGH_TO_TILE) {
            Vector3 moveDirection = new Vector3(facingDirection.x, 0, facingDirection.y);
            transform.position = transform.position + (moveDirection * WALKSPEED * Time.deltaTime);
        } else {
            transform.position = new Vector3(myPath[0].transform.position.x, transform.position.y, myPath[0].transform.position.z);
            if (myPath.Count > 1) {
                FaceDirection(myPath[1].previous.GetDirectionFrom(myPath[1]));
            } else {
                FinishWalking();
            }
            myPath.RemoveAt(0);
        }
    }

    public void UpdateForceMovement() {
        if (!IsBeingForceMoved()) {
            return;
        }

        Vector3 my2DPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 target2DPosition = new Vector3(forceMovedTile.transform.position.x, 0, forceMovedTile.transform.position.z);
        float distanceToNode = Vector3.Distance(my2DPosition, target2DPosition);

        if (distanceToNode > 2f) {
            Vector3 newPosition = Vector3.Lerp(my2DPosition, target2DPosition, 2f * Time.deltaTime);
            newPosition.y = transform.position.y;
            transform.position = newPosition;
        } else {
            target2DPosition.y = transform.position.y;
            transform.position = target2DPosition;
            forceMovedTile = null;
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
                    SetPath(nextAction.moveTiles);
                    break;

                case ActionType.ATTACK:
                    AttackTarget(nextAction.effectedNodes, nextAction.ability);
                    break;
            }
        }
    }

    public void SetPath(List<Tile> path) {
        myTile.MyUnit = null;
        myPath = path;
        myPath[myPath.Count - 1].MyUnit = this;
        myTile = myPath[myPath.Count - 1];
        FaceDirection(myPath[0].previous.GetDirectionFrom(myPath[0]));
        SetWalking(true);
        myManager.UnitStartedMoving();
        myCounters.TilesMoved += path.Count;
    }

    private void FinishWalking() {
        //anim.IsWalking(false);

        RunNextAction(true);
        myManager.UnitFinishedMoving(this);
    }

    public void AttackTarget(List<Node> targetNodes, AttackAction action) {
        currentAbilityTarget = targetNodes[0];
        if (action.areaOfEffect == AreaOfEffect.SINGLE) {
            action.UseAbility(this, currentAbilityTarget);
            FaceDirection(GetDirectionToTile(targetNodes[0]));
        } else {
            action.UseAbility(targetNodes, currentAbilityTarget);
        }
        myManager.UnitStartedAttacking();
        activeAction = action;
        SetAttacking(true);
        myDialogController.Attacking();
        // TODO this doesnt work for multi-targets
        effectsToCreate = action.eventActions.FindAll(e => e.GetType() == typeof(VisualEffectEventAction)).Count;
        projectilesToCreate = action.eventActions.FindAll(e => e.GetType() == typeof(ProjectileEventAction)).Count;
        PlayRandomAttackSound();

        StartCoroutine(AttackRoutine());
    }

    public void PlayRandomAttackSound() {
        if (myStats.attackSFX.Length > 0) {
            AudioClip selectedAudio = myStats.attackSFX[UnityEngine.Random.Range(0, myStats.attackSFX.Length)];
            PlayVoiceClip(selectedAudio);
        }
    }

    public void PlayRandomDeathSound() {
        if (myStats.deathSFX.Length > 0) {
            AudioClip selectedAudio = myStats.deathSFX[UnityEngine.Random.Range(0, myStats.deathSFX.Length)];
            PlayVoiceClip(selectedAudio);
        }
    }

    public void PlayRandomWoundSound() {
        if (myStats.woundSFX.Length > 0) {
            AudioClip selectedAudio = myStats.woundSFX[UnityEngine.Random.Range(0, myStats.woundSFX.Length)];
            PlayVoiceClip(selectedAudio);
        }
    }

    private void PlayVoiceClip(AudioClip voiceClip) {
        PlayOptions attackSoundOptions = new PlayOptions(voiceClip, transform);
        attackSoundOptions.audioMixer = AudioMixers.SFX;
        attackSoundOptions.pitch = UnityEngine.Random.Range(0.8f, 1.2f);

        AudioManager.instance.Play(attackSoundOptions);
    }

    public bool getAttackAnimationPlaying() {
        return false;
    }

    public bool getAttackHasLanded() {
        return true;
    }

    public IEnumerator AttackRoutine() {
        yield return new WaitForSeconds(0.33f);

        //make sure projects have been destroyed
        yield return new WaitUntil(() => getAttackHasLanded() && projectilesToCreate <= 0 && projectiles.Count < 1);
        RunAbilityTargets();

        //wait for effects to end
        yield return new WaitUntil(() => !getAttackAnimationPlaying() && effectsToCreate <= 0 && abilityEffects.Count < 1);

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
        foreach (AbilityTarget abilityTarget in abilityTargets) {
            abilityTarget.abilityFunction();
            activeAction.eventActions.ForEach((eventAction) => {
                if (eventAction.eventTrigger == AbilityEvent.CAST_END && eventAction.eventTarget == EventTarget.TARGETUNIT) {
                    eventAction.action(this, abilityTarget.targetNode);
                }
            });
        }

        // for each effect for node and caster, run now
        activeAction.eventActions.ForEach((eventAction) => {
            if (eventAction.eventTrigger == AbilityEvent.CAST_END) {
                if (eventAction.eventTarget == EventTarget.CASTER || eventAction.eventTarget == EventTarget.TARGETEDTILE) {
                    eventAction.action(this, currentAbilityTarget);
                }
            }
        });
    }

    public void ClearAbilityTargets() {
        abilityTargets.Clear();
        effectsToCreate = 0;
        projectilesToCreate = 0;
        abilityEffects.Clear();
    }

    public void Pull(Tile pullOrigin, int distance) {
        ForceMove(pullOrigin, distance, true);
    }

    public void Push(Tile pushOrigin, int distance) {
        ForceMove(pushOrigin, distance, false);
    }

    private void ForceMove(Tile origin, int distance, bool isPull) {
        Tile tileToMoveTo = FindForceMoveTile(origin, distance, isPull);

        if (tileToMoveTo == null) {
            return;
        }

        forceMovedTile = tileToMoveTo;
        myTile.MyUnit = null;
        forceMovedTile.MyUnit = this;
        myTile = forceMovedTile;
    }

    private Tile FindForceMoveTile(Tile origin, int distance, bool isPull) {
        ReachableTiles tiles = TileMap.instance.pathfinder.findReachableTiles(myTile, distance, WalkableLevel.Walkable, new PathSearchOptions(false, false, myPlayer.faction, myStats.size));
        Vector2 forceDirection = new Vector2(origin.x - myTile.x, origin.y - myTile.y).normalized;
        forceDirection = isPull ? forceDirection : -forceDirection;

        // No tiles to move to
        if (tiles.basic.Count == 0) {
            return null;
        }

        return tiles.basic.Keys.ToList().Aggregate((n1, n2) => {
            int n1Distance = n1.GridDistanceTo(origin);
            int n2Distance = n2.GridDistanceTo(origin);

            if (n1Distance == n2Distance) {
                //compare direction from node
                Vector2 n1Direction = new Vector2(n1.x - myTile.x, n1.y - myTile.y).normalized;
                Vector2 n2Direction = new Vector2(n2.x - myTile.x, n2.y - myTile.y).normalized;

                if (Vector2.Distance(n1Direction, forceDirection) == Vector2.Distance(n2Direction, forceDirection)) {
                    Vector2 absoluteFacing = new Vector2(Mathf.Sign(facingDirection.x), Mathf.Sign(facingDirection.y));

                    Vector2 n1Neighbour = n1.previous.GetDirectionFrom(n1);
                    Vector2 n1AbsoluteDirection = new Vector2(Mathf.Sign(n1Neighbour.x), Mathf.Sign(n1Neighbour.y));

                    Vector2 n2Neighbour = n1.previous.GetDirectionFrom(n2);
                    Vector2 n2AbsoluteDirection = new Vector2(Mathf.Sign(n2Neighbour.x), Mathf.Sign(n2Neighbour.y));

                    return n1AbsoluteDirection.x == absoluteFacing.x && n1AbsoluteDirection.y == absoluteFacing.y ? n1 : n2;
                }

                return Vector2.Distance(n1Direction, forceDirection) < Vector2.Distance(n2Direction, forceDirection) ? n1 : n2;
            }

            if (isPull) {
                return n1Distance < n2Distance ? n1 : n2;
            } else {
                return n1Distance > n2Distance ? n1 : n2;
            }
        });
    }

    public void ApplyBuff(Buff buff) {
        CreateBasicText(buff.name);
        bool buffAdded = myStats.ApplyBuff(buff);
        if (buffAdded && buff.persistentFxPrefab) {
            CreateBuffEffect(buff);
        }
    }

    public void Dispell(bool debuff) {
        if (myStats.buffs.GetBuffs().Count > 0) {
            Buff buffToDispell = myStats.buffs.FindNewestBuff(debuff);
            if (buffToDispell != null) {
                myStats.RemoveBuff(buffToDispell, true);
            }
        }
    }

    public void DispellAll(bool debuff) {
        List<Buff> buffsToDispell = myStats.buffs.FindBuffs(debuff);
        myStats.RemoveBuffs(buffsToDispell);
    }

    public void Summon(Node targetNode, UnitObject unitStats) {
        // TODO work out a clean way of getting allied player
        Player owningPlayer = myPlayer.ai ? myPlayer : PlayerManager.instance.GetPlayer(1);
        UnitManager.instance.SpawnUnit(unitStats, owningPlayer, targetNode.x, targetNode.y);
    }

    public void CreateBuffEffect(Buff buff) {
        GameObject myEffect = Instantiate(buff.persistentFxPrefab);
        myEffect.transform.SetParent(transform.Find("Token"), false);
        buff.persistentFx = myEffect;
    }

    public void CreateEffect(EffectOptions effectOptions) {
        StartCoroutine(CreateEffectRoutine(effectOptions));
    }

    public IEnumerator CreateEffectRoutine(EffectOptions effectOptions) {
        //TODO check what the default value should be
        Node location = effectOptions.location != null ? effectOptions.location : myTile.Nodes.First();

        yield return new WaitForSeconds(effectOptions.delay);
        Transform spawnTransform = location.MyUnit != null ? location.MyUnit.transform.Find("Token") : location.transform;
        GameObject myEffect = Instantiate(effectOptions.effect, spawnTransform);
        if (!effectOptions.rotateWithCharacter) {
            myEffect.transform.rotation = effectOptions.effect.transform.rotation;
        }
        myEffect.transform.SetParent(location.transform, true);
        myEffect.GetComponent<SpriteFxController>().Initialise(this);
        abilityEffects.Add(myEffect);
        effectsToCreate--;
    }

    public void RemoveEffect(GameObject effectToRemove) {
        abilityEffects.Remove(effectToRemove);
    }

    public IEnumerator CreateProjectile(GameObject projectile, Node targetedNode, float speed, float delay = 0) {
        yield return new WaitForSeconds(delay);
        GameObject createdProjectile = Instantiate(projectile);

        ProjectileController createdProjectileController = createdProjectile.GetComponent<ProjectileController>();
        createdProjectileController.SetTarget(this, targetedNode, speed);
        projectiles.Add(createdProjectileController);
        projectilesToCreate--;
    }

    public void CreateProjectileWithDelay(GameObject projectile, Node targetedNode, float speed, float delay) {
        StartCoroutine(CreateProjectile(projectile, targetedNode, speed, delay));
    }

    public void ProjectileHit(ProjectileController projectile) {
        projectiles.Remove(projectile);
    }

    public void CreateBasicText(string text) {
        unitCanvasController.CreateBasicText(text);
    }
}