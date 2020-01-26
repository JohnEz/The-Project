using UnityEngine;
using System.Collections;

public enum MonsterMoveType {
    TOWARDS_TARGET,
    AWAY_FROM_TARGET,
    NONE
}

public enum MonsterActionType {
    MOVE,
    ATTACK,
    NONE
}

public class MonsterAction : ScriptableObject {

    public virtual MonsterActionType ActionType {
        get { return MonsterActionType.NONE; }
    }

    public virtual MonsterMoveType MoveType {
        get { return MonsterMoveType.NONE; }
    }

    public virtual void Instantiate(UnitController myUnit) {
    }
}