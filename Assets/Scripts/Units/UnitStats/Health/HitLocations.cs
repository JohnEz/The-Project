using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New hit locations", menuName = "Unit/Hit Locations")]
public class HitLocations : ScriptableObject {
    public List<HitLocation> myLocations;

    public bool CanBeHitBy(DamageType damageType) {
        return myLocations.Exists((HitLocation location) => location.CanBeHitBy(damageType));
    }

    public HitLocationData RandomHitLocation(DamageType damageType) {
        List<HitLocation> hitLocations = myLocations.FindAll((HitLocation location) => location.CanBeHitBy(damageType));
        int index = Random.Range(0, hitLocations.Count);
        return hitLocations[index].GetData(damageType);
    }
}