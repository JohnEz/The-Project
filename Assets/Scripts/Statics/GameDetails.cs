﻿using System.Collections.Generic;

public static class GameDetails {
    private static string mapName = "";
    private static string playerCharacter = "Criminal";
    private static List<string> playerDeck = BasicDecks.starterElementalist;

    public static string PlayerCharacter {
        get {
            return playerCharacter;
        }
        set {
            playerCharacter = value;
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

    public static List<string> PlayerDeck {
        get {
            return playerDeck;
        }
        set {
            playerDeck = value;
        }
    }
}