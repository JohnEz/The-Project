using System;
using UnityEngine;
using System.Linq;

public enum StatisticTypes {
    NONE,
    CARDS_DRAWN,
    CARDS_PLAYED,
    TILES_MOVED
}

public class UnitStatistics {
    
    private int[] statistics;

    public UnitStatistics() {
        statistics = new int[Enum.GetNames(typeof(StatisticTypes)).Length];
    }

    public int CardsDrawn {
        get { return statistics[(int)StatisticTypes.CARDS_DRAWN]; }
        set { statistics[(int)StatisticTypes.CARDS_DRAWN] = value; }
    }

    public int CardsPlayed {
        get { return statistics[(int)StatisticTypes.CARDS_PLAYED]; }
        set { statistics[(int)StatisticTypes.CARDS_PLAYED] = value; }
    }

    public int TilesMoved {
        get { return statistics[(int)StatisticTypes.TILES_MOVED]; }
        set { statistics[(int)StatisticTypes.TILES_MOVED] = value; }
    }

    public int GetStatistic(StatisticTypes type) {
        return statistics[(int)type];
    }

    public void StartGame() {
        ResetStats();
    }

    public void NewTurn() {
        
    }

    public void EndTurn() {
        ResetStats();
    }

    private void ResetStats() {
        for(int i=0; i<statistics.Length; i++) {
            statistics[i] = 0;
        }
    }

    public override string ToString() {
        return "Cards Drawn: " + CardsDrawn +
            " Cards Played: " + CardsPlayed +
            " Tiles Moved: " + TilesMoved;
    }
}