using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class UnitEquipment {
    public ItemInfo[] items = new ItemInfo[(int)EquipmentSlotType.SIZE];

    public int GetModifiedStat(Stats stat) {
        int value = 0;

        foreach (ItemInfo item in items) {
            value += GetStatValueFromItem(item, stat);
        }
        return value;
    }

    public ItemInfo GetItemInSlot(EquipmentSlotType slotType) {
        return items[(int)slotType];
    }

    #region static

    public static int GetStatValueFromItem(ItemInfo item, Stats stat) {
        if (item == null) {
            return 0;
        }

        switch (stat) {
            case Stats.STRENGTH: return item.strength;
            case Stats.AGILITY: return item.agility;
            case Stats.CONSTITUTION: return item.constitution;
            case Stats.WISDOM: return item.wisdom;
            case Stats.INTELLIGENCE: return item.intelligence;
            case Stats.SPEED: return item.speed;
            case Stats.AC: return item.armour;
            case Stats.AP: return item.actionPoints;
        }

        return 0;
    }

    #endregion static
}