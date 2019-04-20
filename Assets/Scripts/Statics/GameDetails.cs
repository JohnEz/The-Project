using System.Collections.Generic;

public static class GameDetails {
    private static string mapName;
    private static int maxPartySize = 3;
    private static List<UnitObject> party = new List<UnitObject>();

    public static string MapName {
        get {
            return mapName;
        }
        set {
            mapName = value;
        }
    }

    public static int MaxPartySize {
        get {
            return maxPartySize;
        }
        set {
            maxPartySize = value;
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