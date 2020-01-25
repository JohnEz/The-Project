using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public struct HitLocationData {
    public DamageType damageType;
    public float majorInjuryChance;
    public List<Injury> minorInjuries;
    public List<Injury> majorInjuries;
}

[Serializable]
[CreateAssetMenu(fileName = "New hit location", menuName = "Unit/Hit Location")]
public class HitLocation : ScriptableObject {
    public string locationName;
    public List<HitLocationData> data;

    public bool CanBeHitBy(DamageType damageType) {
        return data.Exists((HitLocationData data) => data.damageType == damageType);
    }

    public HitLocationData GetData(DamageType damageType) {
        return data.Find((HitLocationData data) => data.damageType == damageType);
    }
}