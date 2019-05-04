using System.Collections.Generic;
using UnityEngine;

public enum AbilityState {
    NONE,
    MOVEMENT,
    SELECTED,
    INPROGRESS
}

public class UnitSelectionManager : MonoBehaviour {
    public static UnitSelectionManager instance;

    public UnitController CurrentPlayer { get; private set; }
    public UnitController SelectedUnit { get; private set; }

    private AbilityState abilityState = AbilityState.NONE;

    private void Awake() {
        instance = this;
    }

    public Ability ActiveAbility { get; set; }

    public int CurrentActionIndex { get; set; } = 0;

    // Using Ability
    /////////////////
    public bool IsAnAbilitySelected() {
        return abilityState == AbilityState.SELECTED;
    }

    public bool IsAnAbilityInprogress() {
        return abilityState == AbilityState.INPROGRESS;
    }

    public bool IsAnAbilityActive() {
        return abilityState != AbilityState.NONE && abilityState != AbilityState.MOVEMENT;
    }

    public bool IsDisplayingMovement() {
        return abilityState == AbilityState.MOVEMENT;
    }

    // When the user clicked an action from the ability
    public void AbilityInprogress() {
        abilityState = AbilityState.INPROGRESS;
    }

    public void DisplayMovement() {
        abilityState = AbilityState.MOVEMENT;
    }

    public bool CanDisplayMovement() {
        return abilityState != AbilityState.INPROGRESS;
    }

    public bool CanUseAbility() {
        return
            !IsAnAbilityInprogress() &&
            TurnManager.instance.CurrentPhase == TurnPhase.WAITING_FOR_INPUT &&
            TurnManager.instance.IsPlayersTurn();
    }

    public bool CanUseAbility(Ability ability) {
        return CanUseAbility() && ability.caster.ActionPoints >= ability.actionPointCost;
    }

    public void UseAbility(Ability usedAbility) {
        abilityState = AbilityState.SELECTED;
        ActiveAbility = usedAbility;
        UserInterfaceManager.instance.RunNextAbilityAction(usedAbility, CurrentActionIndex);
    }

    public void CancelCurrentAbility() {
        if (IsAnAbilitySelected()) {
            abilityState = AbilityState.NONE;
            UserInterfaceManager.instance.UnshowAbility();
            ActiveAbility = null;
        }
    }

    public void FinishedUsingAbility() {
        if (ActiveAbility) {
            ActiveAbility.SetOnCooldown(true);
            ActiveAbility = null;
        }
        abilityState = AbilityState.NONE;
        CurrentActionIndex = 0;
    }

    public void SelectUnit(UnitController unitToSelect) {
        SelectedUnit = unitToSelect;
    }

    public void UnselectUnit() {
        SelectedUnit = null;
    }
}