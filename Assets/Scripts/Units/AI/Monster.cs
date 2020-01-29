using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MonsterTarget {
    FOCUSED,
    FACING,
    CLOSEST,
    BEHIND,
    ALL,
    NONE,
}

[CreateAssetMenu(fileName = "New Monster", menuName = "Monster/New Monster")]
public class Monster : UnitObject {
    public MonsterAI defaultTurn;

    public List<MonsterAI> myTurns;

    [HideInInspector]
    public UnitController currentTarget;

    [HideInInspector]
    public UnitController focusedTarget;

    public override void Reset(UnitController myUnit = null) {
        base.Reset(myUnit);

        // TODO i should only do this once
        myTurns.ForEach(turn => {
            turn.Instantiate(myUnit);
        });
    }
}