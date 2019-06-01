using System;
using System.Collections.Generic;
using UnityEngine.Events;

public static class GameDetails {
    private static LevelObject level;
    private static List<UnitObject> party = new List<UnitObject>();

    [Serializable] public class OnLevelChange : UnityEvent<LevelObject> { }

    public static OnLevelChange onLevelChange = new OnLevelChange();

    public static LevelObject Level {
        get {
            return level;
        }
        set {
            level = value;
            if (onLevelChange != null) {
                onLevelChange.Invoke(level);
            }
        }
    }

    public static int MaxPartySize {
        get {
            return level != null ? level.maxCharacters : 0;
        }
    }

    public static List<UnitObject> Party {
        get {
            return party;
        }
        set {
            party = value;
        }
    }
}