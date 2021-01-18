using System.Collections;
using DuloGames.UI;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine;

public class ItemSlot : UISlotBase, IntrItemSlot {
    [SerializeField] private UIItemSlot_Group slotGroup = UIItemSlot_Group.None;
    [SerializeField] private int id = 0;

    [Serializable] public class OnAssignEvent : UnityEvent<ItemSlot> { }

    [Serializable] public class OnAssignWithSourceEvent : UnityEvent<ItemSlot, UnityEngine.Object> { }

    [Serializable] public class OnUnassignEvent : UnityEvent<ItemSlot> { }

    public UIItemSlot_Group SlotGroup {
        get { return this.slotGroup; }
        set { this.slotGroup = value; }
    }

    public int ID {
        get { return this.id; }
        set { this.id = value; }
    }

    public OnAssignEvent onAssign = new OnAssignEvent();

    public OnAssignWithSourceEvent onAssignWithSource = new OnAssignWithSourceEvent();

    public OnUnassignEvent onUnassign = new OnUnassignEvent();

    private ItemInfo m_ItemInfo;

    public ItemInfo GetItemInfo() {
        return this.m_ItemInfo;
    }

    public bool Assign(ItemInfo itemInfo) {
        return this.Assign(itemInfo, null);
    }

    public bool Assign(ItemInfo itemInfo, UnityEngine.Object source) {
        if (itemInfo == null)
            return false;

        // Make sure we unassign first, so the event is called before new assignment
        this.Unassign();

        // Use the base class assign to set the icon
        this.Assign(itemInfo.icon);

        this.m_ItemInfo = itemInfo;

        Debug.Log(this.m_ItemInfo);

        // Invoke the on assign event
        if (this.onAssign != null)
            this.onAssign.Invoke(this);

        // Invoke the on assign event
        if (this.onAssignWithSource != null)
            this.onAssignWithSource.Invoke(this, source);

        // Success
        return true;
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

    public static List<ItemSlot> GetSlotsWithID(int ID) {
        List<ItemSlot> slots = new List<ItemSlot>();
        ItemSlot[] sl = Resources.FindObjectsOfTypeAll<ItemSlot>();

        foreach (ItemSlot s in sl) {
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
        string str = "UNDEFINED_EQUIP_TYPE";

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
        string str = "UNDEFINED_WEAPON_TYPE";

        switch (type) {
            case WeaponType.Dagger: str = "Dagger"; break;
            case WeaponType.Mace: str = "Mace"; break;
            case WeaponType.Shield: str = "Shield"; break;
            case WeaponType.Sword: str = "Sword"; break;
            case WeaponType.Bow: str = "Bow"; break;
        }

        return str;
    }

    #endregion Static Methods
}