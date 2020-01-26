using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New move to target", menuName = "Monster/Movement/Move to target")]
public class MoveToTargetMonsterAction : MonsterAction {

    public override MonsterActionType ActionType {
        get { return MonsterActionType.MOVE; }
    }

    public override MonsterMoveType MoveType {
        get { return MonsterMoveType.TOWARDS_TARGET; }
    }
}