using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New AI", menuName = "Monster/New AI")]
public class MonsterAI : ScriptableObject {
    public Sprite telegraph;

    public List<MonsterTarget> targetPriority;

    public List<MonsterAction> actions;

    public void Instantiate(UnitController myUnit) {
        actions.ForEach(action => {
            action.Instantiate(myUnit);
        });
    }
}