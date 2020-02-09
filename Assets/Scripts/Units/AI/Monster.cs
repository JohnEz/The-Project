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
    LAST_INJURY
}

[CreateAssetMenu(fileName = "New Monster", menuName = "Monster/New Monster")]
public class Monster : UnitObject {
    public MonsterAI defaultTurn;

    [SerializeField]
    protected List<MonsterAI> myTurns;

    [HideInInspector]
    public UnitController currentTarget;

    [HideInInspector]
    public UnitController focusedTarget;

    [HideInInspector]
    public UnitController lastInjuryTarget;

    public Queue<MonsterAI> TurnQueue { get; set; }

    public override void Reset(UnitController myUnit = null) {
        base.Reset(myUnit);

        // TODO i should only do this once
        myTurns.ForEach(turn => {
            turn.Instantiate(myUnit);
        });

        TurnQueue = new Queue<MonsterAI>();
        ShuffleTurnQueue();
    }

    public void ShuffleTurnQueue() {
        TurnQueue.Clear();
        List<MonsterAI> remainingTurns = new List<MonsterAI>();
        remainingTurns.AddRange(myTurns);

        int n = remainingTurns.Count;
        while (n > 0) {
            n--;
            int index = Random.Range(0, n);
            TurnQueue.Enqueue(remainingTurns[index]);
            remainingTurns.RemoveAt(index);
        }
    }

    public override void UpdateHitLocation(HitLocation location, UnitController sourceUnit) {
        base.UpdateHitLocation(location, sourceUnit);

        if (sourceUnit) {
            lastInjuryTarget = sourceUnit;
        }
    }

    public MonsterAI GetNextTurn() {
        if (TurnQueue.Count == 0) {
            ShuffleTurnQueue();
        }

        return TurnQueue.Dequeue();
    }

    public MonsterAI PeekNextTurn() {
        if (TurnQueue.Count == 0) {
            ShuffleTurnQueue();
        }

        return TurnQueue.Peek();
    }
}