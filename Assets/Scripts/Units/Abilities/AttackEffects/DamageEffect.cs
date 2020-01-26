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

    public override void AbilityEffect(UnitController caster, Node targetNode) {
        base.AbilityEffect(caster, targetNode);

        //check where it is

        HitLocationData location = targetNode.MyUnit.myStats.GetRandomHitLocation(myType);

        //work out injury
        float majorInjuryChance = ((float)caster.myStats.Strength / (float)targetNode.MyUnit.myStats.Constitution) * location.majorInjuryChance;
        float injuryRoll = Random.value;
        bool isMajorInjury = injuryRoll <= majorInjuryChance;

        Injury injury;

        if (isMajorInjury) {
            injury = location.MajorInjuries.First();
        } else {
            injury = location.MinorInjuries.First();
        }

        targetNode.MyUnit.TakeInjury(injury);
    }
}