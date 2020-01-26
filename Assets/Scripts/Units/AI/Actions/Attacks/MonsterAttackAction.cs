using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New move to target", menuName = "Monster/Movement/Basic Attack")]
public class MonsterAttackAction : MonsterAction {
    public AttackAction baseAttack;
    private AttackAction instantiatedAttack;

    public AttackAction Attack {
        get { return instantiatedAttack; }
    }

    public override MonsterActionType ActionType {
        get { return MonsterActionType.ATTACK; }
    }

    public override void Instantiate(UnitController myUnit) {
        instantiatedAttack = Instantiate(baseAttack);
        instantiatedAttack.caster = myUnit;
    }
}