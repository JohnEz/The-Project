using System;
using System.Collections.Generic;
using UnityEngine;

public enum Stats {
    HEALTH,
    SHIELD,
    STAMINA,
    SPEED,
    POWER,
    BLOCK,
    ARMOUR,
    AP,
    DAMAGE,
    HEALING,
    CRIT,
    LIFE_STEAL
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

    public int baseHealth = 100;
    private int currentHealth;
    private int currentShield;
    public int baseStamina = 50;
    private int currentStamina = 0;
    public int basePower = 15;
    public int baseBlock = 0;
    public int baseArmour = 0;
    public int baseActionPoints = 2;
    public int baseCritChance = 5;

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

    public void Initialise(UnitController myUnit) {
        Reset(myUnit);

        // set graphics
        displayToken = unitTokens[UnityEngine.Random.Range(0, unitTokens.Length)];
        Transform tokenTransform = myUnit.transform.Find("Token");

        tokenTransform.Find("FrontSprite").GetComponent<SpriteRenderer>().sprite = displayToken.frontSprite;
        tokenTransform.Find("BackSprite").GetComponent<SpriteRenderer>().sprite = displayToken.backSprite;

        Vector3 tokenPos = tokenTransform.localPosition;
        tokenTransform.localPosition = new Vector3(tokenPos.x, displayToken.frontSprite.rect.height / 10, tokenPos.z);
    }

    public void Reset(UnitController myUnit = null) {
        currentHealth = MaxHealth;
        Shield = 0;
        currentStamina = MaxStamina;

        Buffs.Clear();
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

        foreach (Buff buff in Buffs) {
            flatMods += buff.GetFlatMod((int)stat);
            percentMods *= buff.GetPercentMod((int)stat);
        }

        return (int)((baseValue + flatMods) * percentMods);
    }

    public List<AttackAction> Attacks {
        get { return instantiatedAttacks; }
    }

    public void SetHealth(int health) {
        currentHealth = Mathf.Clamp(health, 0, MaxHealth);
    }

    public void SetStamina(int stamina) {
        //currentStamina = Mathf.Clamp(stamina, -MaxStamina, MaxStamina);
        currentStamina = Mathf.Clamp(stamina, 0, MaxStamina);
    }

    public int ActionPoints {
        get { return actionPoints; }
        set { actionPoints = value; }
    }

    public int Health {
        get { return currentHealth; }
        set { currentHealth = Mathf.Clamp(value, 0, MaxHealth); }
    }

    public int Shield {
        get { return currentShield; }
        set { currentShield = Mathf.Clamp(value, 0, MaxHealth); }
    }

    public int Stamina {
        get { return currentStamina; }
        set { currentStamina = Mathf.Clamp(value, 0, MaxStamina); }
    }

    public int MaxHealth {
        get { return GetModifiedStat(baseHealth, Stats.HEALTH); }
    }

    public int MaxStamina {
        get { return GetModifiedStat(baseStamina, Stats.STAMINA); }
    }

    public int MaxActionPoints {
        get { return GetModifiedStat(baseActionPoints, Stats.AP); }
    }

    public int Block {
        get { return GetModifiedStat(baseBlock, Stats.BLOCK); }
    }

    public int Armour {
        get { return GetModifiedStat(baseArmour, Stats.ARMOUR); }
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

    // Stats have to be int so we divide lifesteal by 100
    public float LifeSteal {
        get { return (float)GetModifiedStat(0, Stats.LIFE_STEAL) / 100; }
    }

    public List<Buff> Buffs { get; set; } = new List<Buff>();

    public Walkable WalkingType {
        get { return walkingType; }
    }

    public void ApplyStartingTurnBuffs(System.Action<int> takeDamage, System.Action<int> takeHealing) {
        int damage = 0;
        int healing = 0;

        Buffs.ForEach((buff) => {
            damage += Mathf.RoundToInt(MaxHealth * (1 - buff.GetPercentMod((int)Stats.DAMAGE)));
            damage += buff.GetFlatMod((int)Stats.DAMAGE);
            healing += Mathf.RoundToInt(MaxHealth * (buff.GetPercentMod((int)Stats.HEALING) - 1));
            healing += buff.GetFlatMod((int)Stats.HEALING);
        });

        if (damage > 0) {
            takeDamage(damage);
        }

        if (healing > 0) {
            takeHealing(healing);
        }
    }

    public void NewTurn() {
        List<Buff> buffsToRemove = new List<Buff>();

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

        Buffs.ForEach((buff) => {
            buff.duration--;
            if (buff.maxDuration != -1 && buff.duration <= 0) {
                buffsToRemove.Add(buff);
            }
        });

        buffsToRemove.ForEach((buff) => {
            RemoveBuff(buff);
        });
    }

    public void EndTurn() {
    }

    public Buff FindOldestBuff(bool debuff) {
        return Buffs.Find(buff => buff.isDebuff = debuff);
    }

    public Buff FindNewestBuff(bool debuff) {
        return Buffs.FindLast(buff => buff.isDebuff == debuff);
    }

    public Buff FindBuff(string name) {
        return Buffs.Find(buff => buff.name == name);
    }

    public List<Buff> FindBuffs(string name) {
        return Buffs.FindAll(buff => buff.name == name);
    }

    public List<Buff> FindBuffs(bool debuff) {
        return Buffs.FindAll(buff => buff.isDebuff == debuff);
    }

    public void RemoveBuff(Buff buff, bool withEffects = true) {
        buff.Remove(withEffects);
        Buffs.Remove(buff);
    }

    public void RemoveBuffs(List<Buff> buffsToRemove, bool withEffects = true) {
        buffsToRemove.ForEach(buff => RemoveBuff(buff, withEffects));
    }

    public bool ApplyBuff(Buff newBuff) {
        Buff currentBuff = FindBuff(newBuff.name);

        if (currentBuff != null) {
            int newDuration = Math.Max(currentBuff.duration, newBuff.maxDuration);
            int newStacks = Math.Min(currentBuff.stacks + 1, currentBuff.maxStack);

            newBuff.maxDuration = newDuration;
            newBuff.duration = newDuration;
            newBuff.stacks = newStacks;

            RemoveBuff(currentBuff, false);
        }

        Buffs.Add(newBuff);

        return true;
    }

    public override string ToString() {
        return "Name: " + characterName + "\n" +
            "Class: " + className + "\n" +
            "level: " + 1 + "\n" +
            "Health: " + Health + "\n";
    }
}