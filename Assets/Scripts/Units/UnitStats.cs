using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public enum Stats {
	HEALTH,
	STAMINA,
	SPEED,
	POWER,
	CRIT,
	BLOCK,
	ARMOUR,
	AP,
	DAMAGE,
	HEALING,
	MANA
}

public class UnitStats : MonoBehaviour {

	//scaling consts
	const float ARMOUR_DAMAGE_REDUCTION = 0.5f;

	//list of buffs and debuffs
	List<Buff> myBuffs = new List<Buff>();

	[SerializeField]
	int baseHealth = 100;
	int currentHealth;
	[SerializeField]
	int baseStamina = 50;
	int currentStamina = 0;
	[SerializeField]
	int baseSpeed = 3;
	[SerializeField]
	int basePower = 10;
	[SerializeField]
	int baseCrit = 5;
	[SerializeField]
	int baseBlock = 0;
	[SerializeField]
	int baseArmour = 0;
	int baseActionPoints = 2;
	int currentActionPoints = 0;

	[SerializeField]
	Walkable baseWalkingType = Walkable.Walkable;

	[System.NonSerialized]
	public Walkable walkingType;

	public void Initialise() {
		//TODO LOAD BASE STATS FROM DATABASE or scripts
		currentHealth = MaxHealth;
        currentStamina = MaxStamina;
	}

	public int GetModifiedStat(int baseValue, Stats stat) {
		int flatMods = 0;
		float percentMods = 1;

		foreach (Buff buff in myBuffs) {
			flatMods += buff.GetFlatMod((int)stat);
			percentMods *= buff.GetPercentMod((int)stat);
		}

		return (int)((baseValue + flatMods) * percentMods );
	}

    public void SetActionPoints(int actionPoints) {
        currentActionPoints = Mathf.Clamp(actionPoints, 0, MaxActionPoints);
    }

    public void SetHealth(int health) {
		currentHealth = Mathf.Clamp (health, 0, MaxHealth);
	}

    public void SetStamina(int stamina) {
        currentStamina = Mathf.Clamp(stamina, -MaxStamina, MaxStamina);
    }

    public int Health {
        get { return currentHealth; }
    }

    public int Stamina {
        get { return currentStamina; }
    }

    public int ActionPoints {
        get { return currentActionPoints; }
    }

    public int MaxHealth {
		get { return GetModifiedStat(baseHealth, Stats.HEALTH); }
	}

	public int MaxStamina {
		get { return GetModifiedStat(baseStamina, Stats.STAMINA); }
	}

	public int Speed {
		get { return GetModifiedStat(baseSpeed, Stats.SPEED); }
	}

	public int Power {
		get { return GetModifiedStat(basePower, Stats.POWER); }
	}

	public int Crit {
		get { return GetModifiedStat(baseCrit, Stats.CRIT); }
	}

	public int Block {
		get { return GetModifiedStat(baseBlock, Stats.BLOCK); }
	}

	public int Armour {
		get { return GetModifiedStat(baseArmour, Stats.ARMOUR); }
	}

	public int DamageReduction {
		get { return (int)(Armour * ARMOUR_DAMAGE_REDUCTION); }
	}

    public int MaxActionPoints {
		get { return GetModifiedStat(baseActionPoints, Stats.AP); }
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

		Buffs.ForEach ((buff) => {
			damage += buff.GetFlatMod((int)Stats.DAMAGE);
			healing += buff.GetFlatMod((int)Stats.HEALING);
		});

		if (damage > 0) {
			takeDamage (damage);
		}

		if (healing > 0) {
			takeHealing (healing);
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
		return Buffs.Find (buff => buff.isDebuff = debuff);
	}

	public Buff FindNewestBuff(bool debuff) {
		return Buffs.FindLast (buff => buff.isDebuff == debuff);
	}

	public Buff FindBuff (string name) {
		return Buffs.Find ((buff) => buff.name == name); 
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

			RemoveBuff (currentBuff, false);
		}

		Buffs.Add (newBuff);

		return true;
	}

}
