﻿using System.Linq;
using UnityEngine;

public enum DamageType {
    BLUDGEONING,
    SLASHING,
    PIERCING,
    ALL,
}

[CreateAssetMenu(fileName = "New damage effect", menuName = "Ability/Attack/Damage/Damage Location")]
public class DamageLocationEffect : AttackEffect {
    public DamageType myType = DamageType.BLUDGEONING;

    public override void AbilityEffect(UnitController caster, Node targetNode) {
        base.AbilityEffect(caster, targetNode);

        HitLocation location = targetNode.MyUnit.myStats.GetRandomHitLocation(myType);

        //TODO fix this
        if (location == null) {
            return;
        }

        ////work out injury
        //float majorInjuryChance = ((float)caster.myStats.Strength / (float)targetNode.MyUnit.myStats.Constitution) * location.majorInjuryChance;
        //float injuryRoll = Random.value;
        //bool isMajorInjury = injuryRoll <= majorInjuryChance;

        //// TODO fix this
        //if ((isMajorInjury && location.HasAvailableMajorWound()) || (!isMajorInjury && !location.HasAvailableMinorWound())) {
        //    injury = location.MajorInjuries.First();
        //} else if (location.HasAvailableMinorWound()) {
        //    injury = location.MinorInjuries.First();
        //}

        targetNode.MyUnit.HitlocationWounded(location, caster);
    }
}