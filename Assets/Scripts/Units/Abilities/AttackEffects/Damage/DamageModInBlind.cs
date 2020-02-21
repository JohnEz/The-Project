using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Damage/Blindspot Mod")]
public class DamageModInBlind : DamageMod {
    public float damageMod = 1f;

    public override float Apply(float damage, UnitController caster, UnitController target) {
        Tile blindSpot = AIManager.GetBlindSpot(target);

        bool inBlindSpot = blindSpot.OverlapsTile(caster.myTile);

        return inBlindSpot ? damage * damageMod : damage;
    }
}