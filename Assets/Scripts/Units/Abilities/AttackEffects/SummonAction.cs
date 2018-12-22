﻿using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Card/Attack/New Summon Action")]
public class SummonAction : AttackEffect {

    public GameObject summonPrefab;

    private void Awake() {
        // TODO maybe there is a better way to do this? stop it from running
        // run validation on attack criteria - empty tile etc
    }

    public override void AbilityEffect(UnitController caster, Node targetNode) {
        caster.Summon(targetNode, summonPrefab);
    }

}
