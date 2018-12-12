﻿using UnityEngine;
using System;
using System.Collections.Generic;

public enum Stats {
    HEALTH,
    STAMINA,
    SPEED,
    POWER,
    BLOCK,
    ARMOUR,
    AP,
    DAMAGE,
    HEALING,
    MANA
}

[CreateAssetMenu(fileName = "New Unit", menuName = "Unit")]
public class UnitObject : ScriptableObject {

    public string characterName;
    public string className;
    public Sprite Icon; //TODO THIS SHOULD PROBABLY BE IN THE CLASS OR SOMETHING

    //scaling consts
    const int ACTION_POINTS_TO_STAMINA = 2;

    //list of buffs and debuffs
    List<Buff> myBuffs = new List<Buff>();

    public int baseHealth = 100;
    int currentHealth;
    public int baseStamina = 50;
    int currentStamina = 0;
    public int baseBlock = 0;
    public int baseArmour = 0;

    public AudioClip onHitSfx;
    public AudioClip onDeathSfx;

    //Stats for AI
    public Walkable baseWalkingType = Walkable.Walkable;
    public Walkable walkingType;

    public int baseSpeed = 3;

    public List<AttackAction> baseAttacks;
    [HideInInspector]
    public List<AttackAction> instantiatedAttacks;

    public void Initialise(UnitController myUnit) {
        currentHealth = MaxHealth;
        currentStamina = MaxStamina;

        baseAttacks.ForEach(attack => {
            AttackAction instantaitedAttack = Instantiate(attack);
            instantaitedAttack.caster = myUnit;
            instantiatedAttacks.Add(instantaitedAttack);
        });
    }

    public int GetModifiedStat(int baseValue, Stats stat) {
        int flatMods = 0;
        float percentMods = 1;

        foreach (Buff buff in myBuffs) {
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

    public int Health {
        get { return currentHealth; }
    }

    public int Stamina {
        get { return currentStamina; }
    }

    public int MaxHealth {
        get { return GetModifiedStat(baseHealth, Stats.HEALTH); }
    }

    public int MaxStamina {
        get { return GetModifiedStat(baseStamina, Stats.STAMINA); }
    }

    public int Block {
        get { return GetModifiedStat(baseBlock, Stats.BLOCK); }
    }

    public int Armour {
        get { return GetModifiedStat(baseArmour, Stats.ARMOUR); }
    }

    public int Speed {
        get { return GetModifiedStat(baseSpeed, Stats.SPEED); }
    }

    public List<Buff> Buffs {
        get { return myBuffs; }
        set { myBuffs = value; }
    }

    public Walkable WalkingType {
        get { return walkingType; }
    }

    public void ApplyStartingTurnBuffs(System.Action<int> takeDamage, System.Action<int> takeHealing) {
        int damage = 0;
        int healing = 0;

        Buffs.ForEach((buff) => {
            damage += buff.GetFlatMod((int)Stats.DAMAGE);
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

        Buffs.ForEach((buff) => {
            buff.duration--;
            if (buff.duration <= 0) {
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
        return Buffs.Find((buff) => buff.name == name);
    }

    public void RemoveBuff(Buff buff, bool withEffects = true) {
        buff.Remove(withEffects);
        Buffs.Remove(buff);
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

}