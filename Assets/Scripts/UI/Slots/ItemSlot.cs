using UnityEngine;
using System.Collections;
using DuloGames.UI;
using System.Collections.Generic;

public class ItemSlot : UISlotBase, IntrItemSlot {
    [SerializeField] private UIItemSlot_Group slotGroup = UIItemSlot_Group.None;
    [SerializeField] private int id = 0;

    public UIItemSlot_Group SlotGroup {
        get { return this.slotGroup; }
        set { this.slotGroup = value; }
    }

    public int ID {
        get { return this.id; }
        set { this.id = value; }
    }

    public bool Assign(ItemInfo itemInfo, Object source) {
        throw new System.NotImplementedException();
    }

    public ItemInfo GetItemInfo() {
        throw new System.NotImplementedException();
    }

    #region Static Methods

    public static List<ItemSlot> GetSlots() {
        List<ItemSlot> slots = new List<ItemSlot>();
        ItemSlot[] sl = Resources.FindObjectsOfTypeAll<ItemSlot>();

        foreach (ItemSlot s in sl) {
            // Check if the slow is active in the hierarchy
            if (s.gameObject.activeInHierarchy)
                slots.Add(s);
        }

        return slots;
    }

    public static List<UIItemSlot> GetSlotsWithID(int ID) {
        List<UIItemSlot> slots = new List<UIItemSlot>();
        UIItemSlot[] sl = Resources.FindObjectsOfTypeAll<UIItemSlot>();

        foreach (UIItemSlot s in sl) {
            // Check if the slow is active in the hierarchy
            if (s.gameObject.activeInHierarchy && s.ID == ID)
                slots.Add(s);
        }

        return slots;
    }

    public static List<ItemSlot> GetSlotsInGroup(UIItemSlot_Group group) {
        List<ItemSlot> slots = new List<ItemSlot>();
        ItemSlot[] sl = Resources.FindObjectsOfTypeAll<ItemSlot>();

        foreach (ItemSlot s in sl) {
            // Check if the slow is active in the hierarchy
            if (s.gameObject.activeInHierarchy && s.slotGroup == group)
                slots.Add(s);
        }

        // Sort the slots by id
        slots.Sort(delegate (ItemSlot a, ItemSlot b) {
            return a.ID.CompareTo(b.ID);
        });

        return slots;
    }

    public static ItemSlot GetSlot(int ID, UIItemSlot_Group group) {
        ItemSlot[] sl = Resources.FindObjectsOfTypeAll<ItemSlot>();

        foreach (ItemSlot s in sl) {
            // Check if the slow is active in the hierarchy
            if (s.gameObject.activeInHierarchy && s.ID == ID && s.slotGroup == group)
                return s;
        }

        return null;
    }

    public static void PrepareTooltip(ItemInfo itemInfo) {
        if (itemInfo == null)
            return;

        // Set the tooltip width
        if (UITooltipManager.Instance != null)
            UITooltip.SetWidth(UITooltipManager.Instance.itemTooltipWidth);

        // Set the title and description
        UITooltip.AddTitle("<color=#" + ItemQualityColor.GetHexColor(itemInfo.quality) + ">" + itemInfo.name + "</color>");

        // Spacer
        UITooltip.AddSpacer();

        // Item types
        if (itemInfo.weaponType != WeaponType.None) {
            UITooltip.AddLineColumn(WeaponTypeToString(itemInfo.weaponType), "ItemAttribute");
        }
        UITooltip.AddLineColumn(EquipTypeToString(itemInfo.equipType), "ItemAttribute");

        UITooltip.AddSpacer();

        ItemSlot.AddStatToTooltip("Strength", itemInfo.strength);
        ItemSlot.AddStatToTooltip("Agility", itemInfo.agility);
        ItemSlot.AddStatToTooltip("Constitution", itemInfo.constitution);
        ItemSlot.AddStatToTooltip("Wisdom", itemInfo.wisdom);
        ItemSlot.AddStatToTooltip("Intelligence", itemInfo.intelligence);
        ItemSlot.AddStatToTooltip("Speed", itemInfo.speed);
        ItemSlot.AddStatToTooltip("Action Points", itemInfo.actionPoints);
        ItemSlot.AddStatToTooltip("Armour", itemInfo.armour);

        UITooltip.AddSpacer();

        // Set the item description if not empty
        if (!string.IsNullOrEmpty(itemInfo.description)) {
            UITooltip.AddSpacer();
            UITooltip.AddLine(itemInfo.description, "ItemDescription");
        }
    }

    public static void AddStatToTooltip(string name, int value, bool isPercentage = false) {
        if (value == 0) {
            return;
        }
        string modifier = value < 0 ? "-" : "+";
        string spacer = isPercentage ? "% " : " ";

        UITooltip.AddLine(modifier + value.ToString() + spacer + name, "ItemStat");
    }

    public static string EquipTypeToString(EquipmentType type) {
        string str = "Undefined";

        switch (type) {
            case EquipmentType.Weapon: str = "One Hand"; break;
            case EquipmentType.Weapon_MainHand: str = "Main Hand"; break;
            case EquipmentType.Weapon_OffHand: str = "Off Hand"; break;
            case EquipmentType.Head: str = "Head"; break;
            case EquipmentType.Necklace: str = "Necklace"; break;
            case EquipmentType.Shoulders: str = "Shoulders"; break;
            case EquipmentType.Chest: str = "Chest"; break;
            case EquipmentType.Gloves: str = "Gloves"; break;
            case EquipmentType.Pants: str = "Pants"; break;
            case EquipmentType.Boots: str = "Boots"; break;
            case EquipmentType.Trinket: str = "Trinket"; break;
        }

        return str;
    }

    public static string WeaponTypeToString(WeaponType type) {
        string str = "Undefined";

        switch (type) {
            case WeaponType.Dagger: str = "Dagger"; break;
            case WeaponType.Mace: str = "Mace"; break;
            case WeaponType.Shield: str = "Shield"; break;
            case WeaponType.Sword: str = "Sword"; break;
        }

        return str;
    }

    #endregion Static Methods
}