﻿using System.Collections.Generic;

public static class GameDetails {
    private static string mapName = "starterMap";
    private static string playerCharacter = "Fighter";
    private static List<string> playerDeck = BasicDecks.starterBard;

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