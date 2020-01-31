using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New AI", menuName = "Monster/New AI")]
public class MonsterAI : ScriptableObject {
    public string turnName = "Name not set!";
    public Sprite telegraph;

    public List<MonsterTarget> targetPriority;

    public bool ignoreBlindSpot = false;

    public bool takeAnotherTurn = false;

    public List<MonsterAction> actions;

    public void Instantiate(UnitController myUnit) {
        actions.ForEach(action => {
            action.Instantiate(myUnit);
        });
    }
}