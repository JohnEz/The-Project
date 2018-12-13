using System.Collections.Generic;

public static class MatchDetails {
	private static bool versusAi = true;
    private static string mapName = "";
    private static int unitLimit = 4;
    private static List<UnitObject> playerRoster = new List<UnitObject>();
    private static List<string> playerDeck = new List<string>();

	public static bool VersusAi {
		get {
			return versusAi;
		}
		set {
			versusAi = value;
		}
	}

    public static string MapName {
        get {
            return mapName;
        }
        set {
            mapName = value;
        }
    }

    public static int UnitLimit {
        get {
            return unitLimit;
        }
        set {
            unitLimit = value;
        }
    }

    public static List<UnitObject> PlayerRoster {
        get {
            return playerRoster;
        }
        set {
            playerRoster = value;
        }
    }

    public static List<string> PlayerDeck {
        get {
            return playerDeck;
        }
        set {
            playerDeck = value;
        }
    }
}