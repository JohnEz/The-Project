using DuloGames.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBar : MonoBehaviour {
    public static ActionBar instance;

    private List<UISpellSlot> slots;

    [HideInInspector]
    public UnitController currentlyDisplayedUnit;

    public void Awake() {
        instance = this;
    }

    public void Start() {
        slots = UISpellSlot.GetSlotsInGroup(UISpellSlot_Group.Main_1);
    }

    public void Update() {
        if (currentlyDisplayedUnit != UnitSelectionManager.instance.SelectedUnit) {
            DisplayUnit(UnitSelectionManager.instance.SelectedUnit);
        }
    }

    public void UnselectAbilities() {
        slots.ForEach(slot => {
            slot.Unselect();
        });
    }

    public void SelectAbility(int i) {
        UnselectAbilities();
        slots[i].Select();
    }

    public void DisplayUnit(UnitController unitToDisplay) {
        currentlyDisplayedUnit = unitToDisplay;

        int index = 0;
        slots.ForEach((slot) => {
            if (!unitToDisplay || unitToDisplay.myStats.instantiatedAbilities.Count <= index) {
                slot.Unassign();
                return;
            }
            Ability abilityToDisplay = unitToDisplay.myStats.instantiatedAbilities[index];
            slot.Assign(abilityToDisplay.ToAbilityInfo());

            index++;
        });
    }
}