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
            case Stats.MAX_HEALTH: return item.maxHealth;
            case Stats.MAX_SHIELD: return item.maxShield;
            case Stats.SHIELD: return item.shieldPerTurn;
            case Stats.SPEED: return item.speed;
            case Stats.POWER: return item.power;
            case Stats.BLOCK: return item.block;
            case Stats.AP: return item.actionPoints;
            case Stats.CRIT: return item.critChance;
            case Stats.LIFE_STEAL: return item.lifeSteal;
            case Stats.ACCURRACY: return item.accuracy;
            case Stats.DODGE: return item.dodge;
        }

        return 0;
    }

    #endregion static
}