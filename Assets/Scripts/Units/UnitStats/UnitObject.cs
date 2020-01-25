﻿using System;
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
    SPEED,
    AP,
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

[CreateAssetMenu(fileName = "New Unit", menuName = "Unit")]
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

    public HitLocations myHitLocations;
    public int baseStrength = 1;
    public int baseAgility = 1;
    public int baseConstitution = 1;
    public int baseWisdom = 1;
    public int baseIntelligence = 1;
    public int baseAC = 10;
    public int baseSpeed = 3;
    public int baseActionPoints = 2;

    private int actionPoints;

    //Stats for AI
    public WalkableLevel baseWalkingType = WalkableLevel.Walkable;

    public WalkableLevel walkingType;

    public bool isActive = false;

    [HideInInspector]
    public List<AttackAction> instantiatedAttacks;

    public Ability unarmedAbility;

    public List<Ability> baseAbilities;

    [HideInInspector]
    public List<Ability> instantiatedAbilities;

    [HideInInspector]
    public UnitToken displayToken;

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
        tokenTransform.localPosition = new Vector3(tokenPos.x, displayToken.frontSprite.rect.height / 96, tokenPos.z);

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

    public void Reset(UnitController myUnit = null) {
        buffs.Clear();
        instantiatedAttacks.Clear();
        instantiatedAbilities.Clear();

        ItemInfo mainHandItem = equipment.GetItemInSlot(EquipmentSlotType.MainHand);
        Ability mainHandAbility;
        if (mainHandItem == null || mainHandItem.ability == null) {
            mainHandAbility = Instantiate(unarmedAbility);
        } else {
            mainHandAbility = Instantiate(mainHandItem.ability);
        }

        mainHandAbility.caster = myUnit;
        instantiatedAbilities.Add(mainHandAbility);

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

            OnStatChange();
        }
    }

    public int MaxActionPoints {
        get { return baseActionPoints; }
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

    public int Speed {
        get { return GetModifiedStat(baseSpeed, Stats.SPEED); }
    }

    public WalkableLevel WalkingType {
        get { return walkingType; }
    }

    public void ApplyStartingTurnBuffs(System.Action<int> takeDamage, System.Action<int> takeHealing) {
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

    public HitLocationData GetRandomHitLocation(DamageType damageType) {
        return myHitLocations.RandomHitLocation(damageType);
    }

    public override string ToString() {
        return "Name: " + characterName + "\n" +
            "Class: " + className + "\n" +
            "level: " + 1 + "\n";
    }
}