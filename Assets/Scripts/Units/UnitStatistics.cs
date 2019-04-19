using System;
using UnityEngine;
using System.Linq;

public enum StatisticTypes {
    NONE,
    TILES_MOVED
}

public class UnitStatistics {
    private int[] statistics;

    public UnitStatistics() {
        statistics = new int[Enum.GetNames(typeof(StatisticTypes)).Length];
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
        for (int i = 0; i < statistics.Length; i++) {
            statistics[i] = 0;
        }
    }

    public override string ToString() {
        return "Tiles Moved: " + TilesMoved;
    }
}