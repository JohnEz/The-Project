using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New hit locations", menuName = "Unit/Hit Locations")]
public class HitLocations : ScriptableObject {
    public List<HitLocation> myLocationPrefabs;

    [HideInInspector]
    public List<HitLocation> myLocations;

    public List<HitLocation> woundedLocations;

    public void Initialise() {
        myLocations.Clear();

        myLocationPrefabs.ForEach(prefab => {
            HitLocation hitLocation = Instantiate(prefab);
            hitLocation.Initialise();
            myLocations.Add(hitLocation);
        });
    }

    public void HealLocation(HitLocation location) {
        if (!woundedLocations.Contains(location)) {
            Debug.LogError("Tried to heal a location that wasnt damaged", location);
            return;
        }

        location.status = HitLocationStatus.NONE;

        woundedLocations.Remove(location);
    }

    public void DamageLocation(HitLocation location) {
        if (woundedLocations.Contains(location)) {
            Debug.LogError("Tried to damage a location that was already damaged", location);
            return;
        }

        location.status = HitLocationStatus.DAMAGED;

        woundedLocations.Add(location);
    }

    public bool CanBeHitBy(DamageType damageType) {
        return myLocations.Exists((HitLocation location) => location.CanBeHitBy(damageType));
    }

    public HitLocation RandomHitLocation(DamageType damageType) {
        List<HitLocation> hitLocations = myLocations.FindAll((HitLocation location) => !location.Disabled() && location.CanBeHitBy(damageType));
        HitLocation hitLocation = null;
        if (hitLocations.Count > 0) {
            int index = Random.Range(0, hitLocations.Count);
            hitLocation = hitLocations[index];
        }
        return hitLocation;
    }

    public HitLocation GetRecentWoundedHitLocation() {
        return woundedLocations.Last();
    }

    public int GetModifiedStat(Stats stat) {
        int mod = 0;
        myLocations.ForEach(hitLocation => {
            mod += hitLocation.GetModifiedStat(stat);
        });

        return mod;
    }

    public int GetWoundCount() {
        return myLocations.FindAll(location => location.Disabled()).Count;
    }
}