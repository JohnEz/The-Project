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

    // Stats

    public int maxHealth;
    public int maxShield;
    public int shieldPerTurn;
    public int speed;
    public int power;
    public int block;
    public int actionPoints;
    public int critChance;
    public int lifeSteal;
    public int accuracy;
    public int dodge;
}