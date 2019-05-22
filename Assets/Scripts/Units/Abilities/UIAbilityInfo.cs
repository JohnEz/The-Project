using UnityEngine;
using System;
using DuloGames.UI;

[Serializable]
public class UIAbilityInfo : UISpellInfo {
    public Ability ability;
    public int MaxCooldown;
}