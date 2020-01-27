using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class HitLocationData {
    public DamageType damageType;
    public float majorInjuryChance;
    public List<Injury> minorInjuryPrefabs;
    public List<Injury> majorInjuryPrefabs;

    public List<Injury> MinorInjuries { get; set; }

    public List<Injury> MajorInjuries { get; set; }

    public bool HasMajorInjury() {
        return MajorInjuries.Exists(injury => injury.isActive);
    }

    public bool HasAvailableMinorWound() {
        return MinorInjuries.Exists(injury => !injury.isActive);
    }

    public bool HasAvailableMajorWound() {
        return MajorInjuries.Exists(injury => !injury.isActive);
    }

    public bool HasAvailableWound() {
        return HasAvailableMinorWound() || HasAvailableMajorWound();
    }
}

[Serializable]
[CreateAssetMenu(fileName = "New hit location", menuName = "Unit/Hit Location")]
public class HitLocation : ScriptableObject {
    public string locationName;
    public List<HitLocationData> data;

    public Stats effectedStat;
    public int mod;

    public void Initialise() {
        data.ForEach(location => {
            location.MinorInjuries = new List<Injury>();
            location.MajorInjuries = new List<Injury>();

            foreach (Injury prefab in location.minorInjuryPrefabs) {
                Injury injury = Instantiate(prefab);
                location.MinorInjuries.Add(injury);
            }

            foreach (Injury prefab in location.majorInjuryPrefabs) {
                Injury injury = Instantiate(prefab);
                location.MajorInjuries.Add(injury);
            }
        });
    }

    public bool HasAvailableWounds(DamageType damageType) {
        if (!CanBeHitBy(damageType)) {
            return false;
        }

        return GetData(damageType).HasAvailableWound();
    }

    public bool CanBeHitBy(DamageType damageType) {
        return data.Exists((HitLocationData data) => data.damageType == damageType);
    }

    public HitLocationData GetData(DamageType damageType) {
        return data.Find((HitLocationData data) => data.damageType == damageType);
    }

    public bool HasMajorInjury() {
        return data.Exists(hitLocationData => hitLocationData.HasMajorInjury());
    }

    public int GetModifiedStat(Stats stat) {
        return HasMajorInjury() && stat == effectedStat ? mod : 0;
    }
}