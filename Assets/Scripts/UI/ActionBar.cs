using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBar : MonoBehaviour {

    [HideInInspector]
    public UnitController currentlyDisplayedUnit;

    private List<AbilityIcon> abilityIcons;

    public void Start() {
        abilityIcons = new List<AbilityIcon>(GetComponentsInChildren<AbilityIcon>());
        Debug.Log(abilityIcons.Count);
    }

    public void Update() {
        if (currentlyDisplayedUnit != UnitSelectionManager.instance.SelectedUnit) {
            DisplayUnit(UnitSelectionManager.instance.SelectedUnit);
        }
    }

    public void DisplayUnit(UnitController unitToDisplay) {
        currentlyDisplayedUnit = unitToDisplay;

        int index = 0;
        abilityIcons.ForEach((abilityIcon) => {
            if (!unitToDisplay || unitToDisplay.myStats.instantiatedAbilities.Count <= index) {
                abilityIcon.SetAbility(null);
                return;
            }
            Ability abilityToDisplay = unitToDisplay.myStats.instantiatedAbilities[index];
            abilityIcon.SetAbility(abilityToDisplay);

            index++;
        });
    }
}