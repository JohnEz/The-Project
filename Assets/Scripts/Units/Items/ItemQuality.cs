using UnityEngine;
using System.Collections;
using System;

[Serializable]
public enum ItemQuality : int {
    Poor = 0,
    Common = 1,
    Uncommon = 2,
    Rare = 3,
    Epic = 4,
    Legendary = 5
}

public class ItemQualityColor {
    public const string Poor = "9d9d9dff";
    public const string Common = "ffffffff";
    public const string Uncommon = "1eff00ff";
    public const string Rare = "0070ffff";
    public const string Epic = "a335eeff";
    public const string Legendary = "ff8000ff";

    public static string GetHexColor(ItemQuality quality) {
        switch (quality) {
            case ItemQuality.Poor: return Poor;
            case ItemQuality.Common: return Common;
            case ItemQuality.Uncommon: return Uncommon;
            case ItemQuality.Rare: return Rare;
            case ItemQuality.Epic: return Epic;
            case ItemQuality.Legendary: return Legendary;
        }

        return Poor;
    }

    public static Color GetColor(ItemQuality quality) {
        return CommonColorBuffer.StringToColor(GetHexColor(quality));
    }
}