using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Stats {
    MAX_HEALTH,
    MAX_SHIELD,
    SHIELD,
    SPEED,
    POWER,
    BLOCK,
    AP,
    DAMAGE,
    HEALTH,
    CRIT,
    LIFE_STEAL,
    ACCURRACY,
    DODGE,
}

[Serializable]
public struct UnitToken {
    public Sprite frontSprite;
    public Sprite backSprite;
}

[CreateAssetMenu(fileName = "New Unit", menuName = "Unit")]
public class UnitObject : ScriptableObject {
    public string characterName;
    public string className;
    public Sprite Icon;

    public UnitToken[] unitTokens;

    // Audio
    public AudioClip encounterSFX;

    public AudioClip[] attackSFX;
    public AudioClip[] woundSFX;
    public AudioClip[] deathSFX;

    //scaling consts
    private const int ACTION_POINTS_TO_STAMINA = 2;

    [Serializable] public class OnStatChangeEvent : UnityEvent { }

    public OnStatChangeEvent onStatChange = new OnStatChangeEvent();

    public int baseHealth = 100;
    private int currentHealth;
    private int currentShield;
    public int basePower = 15;
    public int baseBlock = 0;
    public int baseArmour = 0;
    public int baseActionPoints = 2;
    public int baseCritChance = 5;
    public int baseAccuracy = 9;
    public int baseDodge = 10;

    [HideInInspector]
    private int actionPoints;

    //Stats for AI
    public Walkable baseWalkingType = Walkable.Walkable;

    public Walkable walkingType;

    public int baseSpeed = 3;
    public bool isActive = false;

    public List<AttackAction> baseAttacks;

    [HideInInspector]
    public List<AttackAction> instantiatedAttacks;

    public List<Ability> baseAbilities;

    [HideInInspector]
    public List<Ability> instantiatedAbilities;

    [HideInInspector]
    public UnitToken displayToken;

    public UnitBuffs buffs;

    public void Awake() {
        buffs = new UnitBuffs();
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
        tokenTransform.localPosition = new Vector3(tokenPos.x, displayToken.frontSprite.rect.height / 10, tokenPos.z);
    }

    public void Reset(UnitController myUnit = null) {
        currentHealth = MaxHealth;
        Shield = 0;

        buffs.Clear();
        instantiatedAttacks.Clear();
        instantiatedAbilities.Clear();

        baseAttacks.ForEach(attack => {
            AttackAction instantaitedAttack = Instantiate(attack);
            instantaitedAttack.caster = myUnit;
            instantiatedAttacks.Add(instantaitedAttack);
        });

        baseAbilities.ForEach(ability => {
            Ability instantaitedAbility = Instantiate(ability);
            instantaitedAbility.caster = myUnit;
            instantiatedAbilities.Add(instantaitedAbility);
        });
    }

    public int GetModifiedStat(int baseValue, Stats stat) {
        int flatMods = 0;
        float percentMods = 1;

        foreach (Buff buff in buffs.GetBuffs()) {
            flatMods += buff.GetFlatMod((int)stat);
            percentMods *= buff.GetPercentMod((int)stat);
        }
        return (int)((baseValue + flatMods) * percentMods);
    }

    public List<AttackAction> Attacks {
        get { return instantiatedAttacks; }
    }

    public int ActionPoints {
        get { return actionPoints; }
        set {
            actionPoints = value;

            if (this.onStatChange != null)
                this.onStatChange.Invoke();
        }
    }

    public int Health {
        get { return currentHealth; }
        set {
            currentHealth = Mathf.Clamp(value, 0, MaxHealth);

            if (this.onStatChange != null)
                this.onStatChange.Invoke();
        }
    }

    public int Shield {
        get { return currentShield; }
        set {
            currentShield = Mathf.Clamp(value, 0, MaxShield);

            if (this.onStatChange != null)
                this.onStatChange.Invoke();
        }
    }

    public int MaxHealth {
        get { return GetModifiedStat(baseHealth, Stats.MAX_HEALTH); }
    }

    public int MaxShield {
        get { return GetModifiedStat(MaxHealth, Stats.MAX_SHIELD); }
    }

    public int MaxActionPoints {
        get { return GetModifiedStat(baseActionPoints, Stats.AP); }
    }

    public int Block {
        get { return GetModifiedStat(baseBlock, Stats.BLOCK); }
    }

    public int Power {
        get { return GetModifiedStat(basePower, Stats.POWER); }
    }

    public int Speed {
        get { return GetModifiedStat(baseSpeed, Stats.SPEED); }
    }

    public int CritChance {
        get { return GetModifiedStat(baseCritChance, Stats.CRIT); }
    }

    public int LifeSteal {
        get { return GetModifiedStat(0, Stats.LIFE_STEAL); }
    }

    public int Accuracy {
        get { return GetModifiedStat(baseAccuracy, Stats.ACCURRACY); }
    }

    public int Dodge {
        get { return GetModifiedStat(baseDodge, Stats.DODGE); }
    }

    // Stats have to be int so we divide lifesteal by 100
    public float LifeStealAsPercent {
        get { return (float)LifeSteal / 100; }
    }

    public Walkable WalkingType {
        get { return walkingType; }
    }

    public void ApplyStartingTurnBuffs(System.Action<int> takeDamage, System.Action<int> takeHealing) {
        int damage = 0;
        int healing = 0;

        buffs.GetBuffs().ForEach((buff) => {
            damage += Mathf.RoundToInt(MaxHealth * (1 - buff.GetPercentMod((int)Stats.DAMAGE)));
            damage += buff.GetFlatMod((int)Stats.DAMAGE);
            healing += Mathf.RoundToInt(MaxHealth * (buff.GetPercentMod((int)Stats.HEALTH) - 1));
            healing += buff.GetFlatMod((int)Stats.HEALTH);
        });

        if (damage > 0) {
            takeDamage(damage);
        }

        if (healing > 0) {
            takeHealing(healing);
        }
    }

    public void NewTurn() {
        Attacks.ForEach(attack => {
            if (attack.IsOnCooldown()) {
                attack.Cooldown--;
            }
        });

        instantiatedAbilities.ForEach(ability => {
            if (ability.IsOnCooldown()) {
                ability.Cooldown--;
            }
        });

        buffs.NewTurn();

        OnStatChange();
    }

    public void EndTurn() {
    }

    public void OnStatChange() {
        if (this.onStatChange != null)
            this.onStatChange.Invoke();
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

    public override string ToString() {
        return "Name: " + characterName + "\n" +
            "Class: " + className + "\n" +
            "level: " + 1 + "\n" +
            "Health: " + Health + "\n";
    }
}