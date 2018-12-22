using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Card/New Summon Action")]
public class SummonAction : AttackEffect {

    public GameObject summonPrefab;

    private void Awake() {
        // TODO maybe there is a better way to do this? stop it from running

    }

    public override void AbilityEffect(UnitController caster, Node targetNode) {
        caster.Summon(targetNode, summonPrefab);
    }

}
