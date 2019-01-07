using System.Collections.Generic;
using UnityEngine;

public class CardGenerator : MonoBehaviour {
    public GameObject cardPrefab;

    public List<UnitObject> units;

    private void Start() {
        PlayerSchool.Roster = units;

        LoadCards(PlayerSchool.Roster);
    }

    public void LoadCards(List<UnitObject> units) {
        foreach (UnitObject unit in units) {
            GameObject unitCard = Instantiate(cardPrefab, transform);

            unitCard.GetComponent<UnitCard>().myUnit = unit;
        }
    }
}