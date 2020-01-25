using UnityEngine;
using UnityEditor;
using System;

[Serializable]
[CreateAssetMenu(fileName = "New Item", menuName = "Item/New Item")]
public class ItemInfo : ScriptableObject {
    public int id;
    public string name;
    public Sprite icon;
    public string description;

    // item types

    public ItemQuality quality;
    public EquipmentType equipType;
    public WeaponType weaponType;
    public Ability ability;

    // Stats

    public int strength;
    public int agility;
    public int constitution;
    public int wisdom;
    public int intelligence;
    public int armour;
    public int speed;
    public int actionPoints;
}