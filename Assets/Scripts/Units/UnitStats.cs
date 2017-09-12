using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public enum Stats {
	VITALITY,
	INTELLIGENCE,
	SPEED,
	POWER,
	CRIT,
	BLOCK,
	ARMOUR,
	AP,
	HEALTH,
	MANA
}

public class UnitStats : MonoBehaviour {

	//scaling consts
	const int VIT_TO_HP = 10;
	const int INT_TO_MANA = 10;

	//list of buffs and debuffs
	List<Buff> myBuffs = new List<Buff>();

	[SerializeField]
	int baseVitality = 10;
	int currentHealth;
	[SerializeField]
	int baseIntelligence = 10;
	int currentMana;
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
	int actionPoints;

	[SerializeField]
	Walkable baseWalkingType = Walkable.Walkable;

	[System.NonSerialized]
	public Walkable walkingType;

	public void Initailise() {
		//TODO LOAD BASE STATS FROM DATABASE
		currentHealth = MaxHealth;
		currentMana = MaxMana;
	}

	public int GetModifiedStat(int baseValue, Stats stat) {
		int flatMods = 0;
		float percentMods = 1;

		foreach (Buff buff in myBuffs) {
			flatMods += buff.GetFlatMod((int)stat);
			percentMods *= buff.GetPercentMod((int)stat);
		}

		return (int)((baseValue * percentMods) + flatMods);
	}

	public void SetHealth(int health) {
		Health = Mathf.Clamp (health, 0, MaxHealth);
	}

	public int Vitality {
		get { return GetModifiedStat(baseVitality, Stats.VITALITY); }
	}

	public int Intelligence {
		get { return GetModifiedStat(baseIntelligence, Stats.INTELLIGENCE); }
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

	public int MaxHealth {
		get { return Vitality * VIT_TO_HP; }
	}

	public int Health {
		get { return currentHealth; }
		set { currentHealth = value; }
	}

	public int MaxMana {
		get { return Intelligence * INT_TO_MANA; }
	}

	public int Mana {
		get { return currentMana; }
		set { currentMana = value; }
	}

	public int MaxActionPoints {
		get { return GetModifiedStat(baseActionPoints, Stats.AP); }
	}

	public int ActionPoints {
		get { return actionPoints; }
		set { actionPoints = value; }
	}

	public List<Buff> Buffs {
		get { return myBuffs; }
		set { myBuffs = value; }
	}

	public Walkable WalkingType {
		get { return walkingType; }
	}

	public void NewTurn() {
		List<Buff> buffsToRemove = new List<Buff> ();

		Buffs.ForEach ((buff) => {
			buff.duration--;
			if (buff.duration <= 0) {
				buffsToRemove.Add(buff);
			}
		});

		buffsToRemove.ForEach ((buff) => {
			RemoveBuff(buff);
		});
	}

	public void EndTurn() {

	}

	public Buff FindOldestBuff(bool debuff) {
		return Buffs.Find (buff => buff.isDebuff = debuff);
	}

	public Buff FindNewestBuff(bool debuff) {
		return Buffs.FindLast (buff => buff.isDebuff = debuff);
	}

	public void RemoveBuff(Buff buff, bool withEffects = true) {
		buff.Remove(withEffects);
		Buffs.Remove(buff);
	}

	public bool ApplyBuff(Buff newBuff) {
		Buff currentBuff = Buffs.Find ((buff) => buff.name == newBuff.name); 

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
