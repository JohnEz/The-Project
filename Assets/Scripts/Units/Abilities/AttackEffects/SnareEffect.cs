using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Card/Attack/Snare")]
public class SnareEffect : AttackEffect {

    public int duration = 1;

    public GameObject effectPrefab;

    public override void AbilityEffect(UnitController caster, UnitController target) {
        target.ApplyBuff(new Snare(duration, effectPrefab));
    }
}
