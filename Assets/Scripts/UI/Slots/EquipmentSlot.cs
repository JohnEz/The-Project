using UnityEngine;
using System.Collections;
using DuloGames.UI;
using UnityEngine.Events;
using System;
using Object = UnityEngine.Object;
using UnityEngine.UI;
using System.Collections.Generic;

public class EquipmentSlot : UISlotBase, IntrItemSlot {

    [Serializable] public class OnAssignEvent : UnityEvent<EquipmentSlot> { }

    [Serializable] public class OnAssignWithSourceEvent : UnityEvent<EquipmentSlot, Object> { }

    [Serializable] public class OnUnassignEvent : UnityEvent<EquipmentSlot> { }

    public OnAssignEvent onAssign = new OnAssignEvent();
    public OnAssignWithSourceEvent onAssignWithSource = new OnAssignWithSourceEvent();
    public OnUnassignEvent onUnassign = new OnUnassignEvent();

    [SerializeField] private EquipmentSlotType equipSlotType = EquipmentSlotType.MainHand;

    public EquipmentSlotType EquipSlotType {
        get { return this.equipSlotType; }
        set { this.equipSlotType = value; }
    }

    private ItemInfo m_ItemInfo;

    public ItemInfo GetItemInfo() {
        return this.m_ItemInfo;
    }

    public override bool IsAssigned() {
        return (this.m_ItemInfo != null);
    }

    public bool Assign(ItemInfo itemInfo, Object source) {
        if (itemInfo == null)
            return false;

        // Check if the equipment type matches the target slot
        if (!this.CheckEquipType(itemInfo))
            return false;

        // Make sure we unassign first, so the event is called before new assignment
        this.Unassign();

        // Use the base class assign to set the icon
        this.Assign(itemInfo.icon);

        // Set the item info
        this.m_ItemInfo = itemInfo;

        // Invoke the on assign event
        if (this.onAssign != null)
            this.onAssign.Invoke(this);

        // Invoke the on assign event
        if (this.onAssignWithSource != null)
            this.onAssignWithSource.Invoke(this, source);

        // Success
        return true;
    }

    public bool Assign(ItemInfo itemInfo) {
        return this.Assign(itemInfo, null);
    }

    public override bool Assign(Object source) {
        if (source is IntrItemSlot) {
            IntrItemSlot sourceSlot = source as IntrItemSlot;

            if (sourceSlot != null) {
                // Check if the equipment type matches the target slot
                if (!this.CheckEquipType(sourceSlot.GetItemInfo()))
                    return false;

                return this.Assign(sourceSlot.GetItemInfo(), source);
            }
        }

        // Default
        return false;
    }

    public override void Unassign() {
        // Remove the icon
        base.Unassign();

        // Clear the spell info
        this.m_ItemInfo = null;

        // Invoke the on unassign event
        if (this.onUnassign != null)
            this.onUnassign.Invoke(this);
    }

    public virtual bool CheckEquipType(ItemInfo info) {
        if (info == null)
            return false;

        if (!EquipmentSlot.ItemFitsSlot(info.equipType, this.equipSlotType))
            return false;

        return true;
    }

    public override bool CanSwapWith(Object target) {
        if (target is IntrItemSlot) {
            // Check if the equip slot accepts this item
            if (target is EquipmentSlot) {
                return (target as EquipmentSlot).CheckEquipType(this.GetItemInfo());
            }

            // It's an item slot
            return true;
        }

        // Default
        return false;
    }

    public override bool PerformSlotSwap(Object sourceObject) {
        // Get the source slot
        IntrItemSlot sourceSlot = (sourceObject as IntrItemSlot);

        // Get the source item info
        ItemInfo sourceItemInfo = sourceSlot.GetItemInfo();

        // Assign the source slot by this slot
        bool assign1 = sourceSlot.Assign(this.GetItemInfo(), this);

        // Assign this slot by the source slot
        bool assign2 = this.Assign(sourceItemInfo, sourceObject);

        // Return the status
        return (assign1 && assign2);
    }

    public override void OnTooltip(bool show) {
        UITooltip.InstantiateIfNecessary(this.gameObject);

        // Handle unassigned
        if (!this.IsAssigned()) {
            // If we are showing the tooltip
            if (show) {
                UITooltip.AddTitle(EquipmentSlot.EquipSlotTypeToString(this.equipSlotType));
                UITooltip.SetHorizontalFitMode(ContentSizeFitter.FitMode.PreferredSize);
                UITooltip.AnchorToRect(this.transform as RectTransform);
                UITooltip.Show();
            } else {
                UITooltip.Hide();
            }
        } else {
            // Make sure we have spell info, otherwise game might crash
            if (this.m_ItemInfo == null)
                return;

            // If we are showing the tooltip
            if (show) {
                ItemSlot.PrepareTooltip(this.m_ItemInfo);
                UITooltip.AnchorToRect(this.transform as RectTransform);
                UITooltip.Show();
            } else UITooltip.Hide();
        }
    }

    protected override void OnThrowAwayDenied() {
        if (!this.IsAssigned())
            return;

        // Find free inventory slot
        List<ItemSlot> itemSlots = ItemSlot.GetSlotsInGroup(UIItemSlot_Group.Inventory);

        if (itemSlots.Count > 0) {
            // Get the first free one
            foreach (ItemSlot slot in itemSlots) {
                if (!slot.IsAssigned()) {
                    // Assign this equip slot to the item slot
                    slot.Assign(this);

                    // Unassing this equip slot
                    this.Unassign();
                    break;
                }
            }
        }
    }

    #region Static Methods

    public static string EquipSlotTypeToString(EquipmentSlotType type) {
        string str = "Undefined";

        switch (type) {
            case EquipmentSlotType.MainHand: str = "Main Hand"; break;
            case EquipmentSlotType.OffHand: str = "Off Hand"; break;
            case EquipmentSlotType.Head: str = "Head"; break;
            case EquipmentSlotType.Necklace: str = "Necklace"; break;
            case EquipmentSlotType.Shoulders: str = "Shoulders"; break;
            case EquipmentSlotType.Chest: str = "Chest"; break;
            case EquipmentSlotType.Gloves: str = "Gloves"; break;
            case EquipmentSlotType.Pants: str = "Pants"; break;
            case EquipmentSlotType.Boots: str = "Boots"; break;
            case EquipmentSlotType.Trinket: str = "Trinket"; break;
        }

        return str;
    }

    public static bool ItemFitsSlot(EquipmentType eType, EquipmentSlotType sType) {
        switch (sType) {
            case EquipmentSlotType.Boots: return eType == EquipmentType.Boots;
            case EquipmentSlotType.Chest: return eType == EquipmentType.Chest;
            case EquipmentSlotType.Gloves: return eType == EquipmentType.Gloves;
            case EquipmentSlotType.Head: return eType == EquipmentType.Head;
            case EquipmentSlotType.Necklace: return eType == EquipmentType.Necklace;
            case EquipmentSlotType.Pants: return eType == EquipmentType.Pants;
            case EquipmentSlotType.Shoulders: return eType == EquipmentType.Shoulders;
            case EquipmentSlotType.Trinket: return eType == EquipmentType.Trinket;

            case EquipmentSlotType.MainHand:
                return eType == EquipmentType.Weapon_MainHand || eType == EquipmentType.Weapon;

            case EquipmentSlotType.OffHand:
                return eType == EquipmentType.Weapon_OffHand || eType == EquipmentType.Weapon;
        }

        return false;
    }

    public static List<EquipmentSlot> GetSlots() {
        List<EquipmentSlot> slots = new List<EquipmentSlot>();
        EquipmentSlot[] sl = Resources.FindObjectsOfTypeAll<EquipmentSlot>();

        foreach (EquipmentSlot s in sl) {
            // Check if the slow is active in the hierarchy
            if (s.gameObject.activeInHierarchy)
                slots.Add(s);
        }

        return slots;
    }

    public static EquipmentSlot GetSlotWithType(EquipmentSlotType type) {
        EquipmentSlot[] sl = Resources.FindObjectsOfTypeAll<EquipmentSlot>();

        foreach (EquipmentSlot s in sl) {
            // Check if the slow is active in the hierarchy
            if (s.gameObject.activeInHierarchy && s.equipSlotType == type)
                return s;
        }

        // Default
        return null;
    }

    public static List<EquipmentSlot> GetSlotsWithType(EquipmentSlotType type) {
        List<EquipmentSlot> slots = new List<EquipmentSlot>();
        EquipmentSlot[] sl = Resources.FindObjectsOfTypeAll<EquipmentSlot>();

        foreach (EquipmentSlot s in sl) {
            // Check if the slow is active in the hierarchy
            if (s.gameObject.activeInHierarchy && s.equipSlotType == type)
                slots.Add(s);
        }

        return slots;
    }

    #endregion Static Methods
}