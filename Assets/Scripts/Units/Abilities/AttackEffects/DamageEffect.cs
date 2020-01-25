using System.Linq;
using UnityEngine;

public enum DamageType {
    BLUDGEONING,
    SLASHING,
    PIERCING,
}

[CreateAssetMenu(fileName = "New damage effect", menuName = "Ability/Attack/Damage")]
public class DamageEffect : AttackEffect {
    public DamageType myType = DamageType.BLUDGEONING;

    public override void AbilityEffect(UnitController caster, UnitController target) {
        base.AbilityEffect(caster, target);

        //check where it is

        HitLocationData location = target.myStats.GetRandomHitLocation(myType);

        //work out injury
        float majorInjuryChance = ((float)caster.myStats.Strength / (float)target.myStats.Constitution) * location.majorInjuryChance;
        float injuryRoll = Random.value;
        bool isMajorInjury = injuryRoll <= majorInjuryChance;

        Injury injury;

        if (isMajorInjury) {
            injury = location.majorInjuries.First();
        } else {
            injury = location.minorInjuries.First();
        }

        target.CreateBasicText(injury.description);
    }
}