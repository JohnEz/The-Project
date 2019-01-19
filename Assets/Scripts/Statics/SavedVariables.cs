using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SavedVariables {
    public static List<string> encounteredEnemies = new List<string>();

    public static bool HasEncounteredEnemy(string encounteredEnemy) {
        return encounteredEnemies.Contains(encounteredEnemy);
    }

    public static void EncounteredEnemy(string encounteredEnemy) {
        if (!HasEncounteredEnemy(encounteredEnemy)) {
            encounteredEnemies.Add(encounteredEnemy);
        }
    }
}