using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New hit locations", menuName = "Unit/Hit Locations")]
public class HitLocations : ScriptableObject {
    public List<HitLocation> myLocationPrefabs;

    [HideInInspector]
    public List<HitLocation> myLocations;

    public void Initialise() {
        myLocations.Clear();

        myLocationPrefabs.ForEach(prefab => {
            HitLocation hitLocation = Instantiate(prefab);
            hitLocation.Initialise();
            myLocations.Add(hitLocation);
        });
    }

    public bool CanBeHitBy(DamageType damageType) {
        return myLocations.Exists((HitLocation location) => location.CanBeHitBy(damageType));
    }

    public HitLocationData RandomHitLocation(DamageType damageType) {
        List<HitLocation> hitLocations = myLocations.FindAll((HitLocation location) => location.CanBeHitBy(damageType));
        int index = Random.Range(0, hitLocations.Count);
        return hitLocations[index].GetData(damageType);
    }

    public int GetModifiedStat(Stats stat) {
        int mod = 0;
        myLocations.ForEach(hitLocation => {
            mod += hitLocation.GetModifiedStat(stat);
        });

        return mod;
    }
}