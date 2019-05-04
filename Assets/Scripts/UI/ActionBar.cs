using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBar : MonoBehaviour {
    public static ActionBar instance;

    [HideInInspector]
    public UnitController currentlyDisplayedUnit;

    private List<AbilityIcon> abilityIcons;
    public AbilityDescriptionController abilityDescription;

    public void Awake() {
        instance = this;
    }

    public void Start() {
        abilityIcons = new List<AbilityIcon>(GetComponentsInChildren<AbilityIcon>());

        int slot = 0;
        foreach (AbilityIcon icon in abilityIcons) {
            icon.slot = slot;
            slot++;
        }
    }

    public void Update() {
        if (currentlyDisplayedUnit != UnitSelectionManager.instance.SelectedUnit) {
            DisplayUnit(UnitSelectionManager.instance.SelectedUnit);
        }
    }

    public void UnselectAbilities() {
        abilityIcons.ForEach(icon => {
            icon.Unselect();
        });
    }

    public void SelectAbility(int i) {
        UnselectAbilities();
        abilityIcons[i].Select();
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