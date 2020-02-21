using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Stats {
    STRENGTH,
    AGILITY,
    CONSTITUTION,
    WISDOM,
    INTELLIGENCE,
    AC,
    MAX_HEALTH,
    MAX_SHIELD,
    SHIELD,
    SPEED,
    HIT,
    AP,
    MOVE_AP,
}

public enum UnitSize {
    SMALL,
    MEDIUM,
    LARGE
}

[Serializable]
public struct UnitToken {
    public Sprite frontSprite;
    public Sprite backSprite;
}

[CreateAssetMenu(fileName = "New Unit", menuName = "Unit/Unit")]
public class UnitObject : ScriptableObject {
    public string characterName;
    public string className;
    public Sprite Icon;

    public UnitToken[] unitTokens;
    public UnitSize size = UnitSize.SMALL;

    public UnitBuffs buffs;

    public UnitEquipment equipment;

    // Audio
    public AudioClip encounterSFX;

    public AudioClip[] attackSFX;
    public AudioClip[] woundSFX;
    public AudioClip[] deathSFX;

    [Serializable] public class OnStatChangeEvent : UnityEvent { }

    public OnStatChangeEvent onStatChange = new OnStatChangeEvent();

    [Serializable] public class OnHitLocationEvent : UnityEvent { }

    public OnHitLocationEvent onHitLocationChange = new OnHitLocationEvent();

    public HitLocations myHitLocationsPrefab;

    [HideInInspector]
    public HitLocations myHitLocations;

    public int baseHealth = 100;
    private int currentHealth;
    private int currentShield;
    public int baseStrength = 1;
    public int baseAgility = 1;
    public int baseConstitution = 1;
    public int baseWisdom = 1;
    public int baseIntelligence = 1;
    public int baseAC = 10;
    public int baseSpeed = 5;
    public int baseHit = 1;
    public int baseActionPoints = 1;
    public int baseMoveActionPoints = 1;
    public int baseWoundLimit = 3;

    private int moveActionPoints;
    private int actionPoints;

    public WalkableLevel baseWalkingType = WalkableLevel.Walkable;

    public WalkableLevel walkingType;

    [HideInInspector]
    public List<AttackAction> instantiatedAttacks;

    public Ability unarmedAbility;

    public List<Ability> baseAbilities;

    [HideInInspector]
    public List<Ability> instantiatedAbilities;

    [HideInInspector]
    public UnitToken displayToken;

    [HideInInspector]
    public float unitHalfHeight;

    public void Awake() {
        buffs = new UnitBuffs();
        equipment = equipment != null ? equipment : new UnitEquipment();
        Reset();
        displayToken = unitTokens[UnityEngine.Random.Range(0, unitTokens.Length)];
    }

    public void Initialise(UnitController myUnit) {
        Reset(myUnit);

        // set graphics
        Transform tokenTransform = myUnit.transform.Find("Token");

        tokenTransform.Find("FrontSprite").GetComponent<SpriteRenderer>().sprite = displayToken.frontSprite;
        tokenTransform.Find("BackSprite").GetComponent<SpriteRenderer>().sprite = displayToken.backSprite;

        Vector3 tokenPos = tokenTransform.localPosition;

        int pixelsPerUnit = 50;
        float baseHeight = 0.1f;
        unitHalfHeight = (displayToken.frontSprite.rect.height / pixelsPerUnit) / 2;

        tokenTransform.localPosition = new Vector3(tokenPos.x, unitHalfHeight + baseHeight, tokenPos.z);

        myUnit.transform.Find("SmallBase").gameObject.SetActive(false);
        switch (size) {
            case UnitSize.SMALL:
                myUnit.transform.Find("SmallBase").gameObject.SetActive(true);
                break;

            case UnitSize.MEDIUM:
                myUnit.transform.Find("MediumBase").gameObject.SetActive(true);
                break;

            case UnitSize.LARGE:
                myUnit.transform.Find("LargeBase").gameObject.SetActive(true);
                break;
        }
    }

    public virtual void Reset(UnitController myUnit = null) {
        myHitLocations = Instantiate(myHitLocationsPrefab);
        myHitLocations.Initialise();

        currentHealth = MaxHealth;
        Shield = 0;

        buffs.Clear();
        instantiatedAttacks.Clear();
        instantiatedAbilities.Clear();

        ItemInfo mainHandItem = equipment.GetItemInSlot(EquipmentSlotType.MainHand);
        Ability mainHandAbility = null;
        bool hasMainHandAbility = mainHandItem != null && mainHandItem.ability != null;

        if (hasMainHandAbility) {
            mainHandAbility = Instantiate(mainHandItem.ability);
        } else if (unarmedAbility != null) {
            mainHandAbility = Instantiate(unarmedAbility);
        }

        if (mainHandAbility != null) {
            mainHandAbility.caster = myUnit;
            instantiatedAbilities.Add(mainHandAbility);
        } else {
            Debug.Log(className + " doesnt have a main hand ability?!");
        }

        baseAbilities.ForEach(ability => {
            Ability instantaitedAbility = Instantiate(ability);
            instantaitedAbility.caster = myUnit;
            instantiatedAbilities.Add(instantaitedAbility);
        });
    }

    public int GetModifiedStat(int baseValue, Stats stat) {
        int flatMods = 0;
        float percentMods = 1;

        // get stats from equipment
        flatMods += equipment.GetModifiedStat(stat);

        flatMods += myHitLocations.GetModifiedStat(stat);

        foreach (Buff buff in buffs.GetBuffs()) {
            flatMods += buff.GetFlatMod((int)stat);
            percentMods *= buff.GetPercentMod((int)stat);
        }
        return (int)((baseValue + flatMods) * percentMods);
    }

    public List<AttackAction> Attacks {
        get { return instantiatedAttacks; }
    }

    public int MoveActionPoints {
        get { return moveActionPoints; }
        set {
            moveActionPoints = value;

            OnStatChange();
        }
    }

    public int ActionPoints {
        get { return actionPoints; }
        set {
            actionPoints = value;

            OnStatChange();
        }
    }

    public int MaxActionPoints {
        get { return baseActionPoints; }
    }

    public int MaxMoveActionPoints {
        get { return baseMoveActionPoints; }
    }

    public int Strength {
        get { return GetModifiedStat(baseStrength, Stats.STRENGTH); }
    }

    public int Agility {
        get { return GetModifiedStat(baseAgility, Stats.AGILITY); }
    }

    public int Constitution {
        get { return GetModifiedStat(baseConstitution, Stats.CONSTITUTION); }
    }

    public int Wisdom {
        get { return GetModifiedStat(baseWisdom, Stats.WISDOM); }
    }

    public int Intelligence {
        get { return GetModifiedStat(baseIntelligence, Stats.INTELLIGENCE); }
    }

    public int AC {
        get { return GetModifiedStat(baseAC, Stats.AC); }
    }

    public int Health {
        get { return currentHealth; }
        set {
            currentHealth = Mathf.Clamp(value, 0, MaxHealth);

            OnStatChange();
        }
    }

    public int Shield {
        get { return currentShield; }
        set {
            currentShield = Mathf.Clamp(value, 0, MaxShield);

            OnStatChange();
        }
    }

    public int MaxHealth {
        get { return GetModifiedStat(baseHealth, Stats.MAX_HEALTH); }
    }

    public int MaxShield {
        get { return GetModifiedStat(MaxHealth, Stats.MAX_SHIELD); }
    }

    public int Speed {
        get { return GetModifiedStat(baseSpeed, Stats.SPEED); }
    }

    public int Hit {
        get { return GetModifiedStat(baseHit, Stats.HIT); }
    }

    public int GetStat(Stats stat) {
        switch (stat) {
            case Stats.AC: return AC;
            case Stats.AGILITY: return Agility;
            case Stats.AP: return MaxActionPoints;
            case Stats.CONSTITUTION: return Constitution;
            case Stats.INTELLIGENCE: return Intelligence;
            case Stats.SPEED: return Speed;
            case Stats.STRENGTH: return Strength;
            case Stats.WISDOM: return Wisdom;
            case Stats.MAX_HEALTH: return MaxHealth;
            case Stats.MAX_SHIELD: return MaxShield;
            case Stats.SHIELD: return Shield;
            default:
                Debug.LogError("Tried to access unknown stat");
                Debug.LogError(stat);
                return 0;
        }
    }

    public int WoundCount {
        get { return myHitLocations.GetWoundCount(); }
    }

    public WalkableLevel WalkingType {
        get { return walkingType; }
    }

    public void ApplyStartingTurnBuffs(System.Action<int> takeDamage, System.Action<int> takeHealing) {
    }

    public void NewTurn(bool isStunned) {
        instantiatedAbilities.ForEach(ability => {
            if (ability.IsOnCooldown()) {
                ability.Cooldown--;
            }
        });

        if (!isStunned) {
            ActionPoints = MaxActionPoints;
            MoveActionPoints = MaxMoveActionPoints;
        }

        buffs.NewTurn();

        OnStatChange();
    }

    public void EndTurn() {
        ActionPoints = 0;
        MoveActionPoints = 0;
    }

    public void OnStatChange() {
        if (this.onStatChange != null)
            this.onStatChange.Invoke();
    }

    public void OnHitLocationChange() {
        if (this.onHitLocationChange != null)
            this.onHitLocationChange.Invoke();
    }

    public bool ApplyBuff(Buff newBuff) {
        buffs.ApplyBuff(newBuff);

        OnStatChange();

        return true;
    }

    public void RemoveBuff(Buff buff, bool withEffects = true) {
        buffs.RemoveBuff(buff);

        OnStatChange();
    }

    public void RemoveBuffs(List<Buff> buffsToRemove, bool withEffects = true) {
        buffs.RemoveBuffs(buffsToRemove, withEffects);

        OnStatChange();
    }

    public virtual void UpdateHitLocation(HitLocation location, UnitController sourceUnit) {
        OnHitLocationChange();
        OnStatChange();
    }

    public HitLocation GetRandomHitLocation(DamageType damageType) {
        return myHitLocations.RandomHitLocation(damageType);
    }

    public HitLocation GetRecentWoundedHitLocation() {
        return myHitLocations.GetRecentWoundedHitLocation();
    }

    public void DamageHitLocation(HitLocation location, UnitController sourceUnit) {
        myHitLocations.DamageLocation(location);
        UpdateHitLocation(location, sourceUnit);
    }

    public void HealHitLocation(HitLocation location, UnitController sourceUnit) {
        myHitLocations.HealLocation(location);
        UpdateHitLocation(location, sourceUnit);
    }

    public override string ToString() {
        return "Name: " + characterName + "\n" +
            "Class: " + className + "\n" +
            "level: " + 1 + "\n";
    }
}