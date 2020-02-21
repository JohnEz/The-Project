using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum HitLocationStatus {
    NONE,
    RESTRAINED,
    DAMAGED,
}

[Serializable]
[CreateAssetMenu(fileName = "New hit location", menuName = "Unit/Hit Location")]
public class HitLocation : ScriptableObject {
    public string locationName;

    public HitLocationStatus status = HitLocationStatus.NONE;
    public DamageType vunerability = DamageType.ALL;

    // outcome stats
    public Stats effectedStat;

    public int mod;

    public void Initialise() {
    }

    public bool CanBeHitBy(DamageType damageType) {
        return vunerability == DamageType.ALL || damageType == vunerability;
    }

    public bool Disabled() {
        return status != HitLocationStatus.NONE;
    }

    public int GetModifiedStat(Stats stat) {
        return Disabled() && stat == effectedStat ? mod : 0;
    }
}