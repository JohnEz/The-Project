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

    public UnitController selectedUnit;

    public UnitController SelectedUnit {
        get { return selectedUnit; }
    }

    public Ability ActiveAbility { get; set; }

    public int CurrentActionIndex { get; set; } = 0;

    private AbilityState abilityState = AbilityState.NONE;

    private void Awake() {
        instance = this;
    }

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

    public void ResetState() {
        abilityState = AbilityState.NONE;
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
        return CanUseAbility() && ability.caster.myStats.ActionPoints >= ability.actionPointCost;
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
        if (selectedUnit != null) {
            UnselectUnit();
        }

        selectedUnit = unitToSelect;

        if (selectedUnit == null) {
            return;
        }

        HighlightManager.instance.SetEffectedNodes(selectedUnit.myTile.Nodes, SquareTarget.SELECTED_UNIT);
    }

    public void UnselectUnit() {
        if (selectedUnit == null) {
            return;
        }

        HighlightManager.instance.UnhighlightNodes(selectedUnit.myTile.Nodes);
        selectedUnit = null;
    }

    public UnitController SelectNextUnit() {
        UnitController nextUnit = FindNextUnit();
        if (nextUnit) {
            SelectUnit(nextUnit);
            return selectedUnit;
        }

        return null;
    }

    public UnitController FindNextUnit() {
        Player currentPlayer = TurnManager.instance.GetCurrentPlayer();
        List<UnitController> playerUnits = UnitManager.instance.GetPlayersUnits(currentPlayer.id);
        int currentSelectedIndex = selectedUnit != null ? playerUnits.IndexOf(selectedUnit) : -1;

        int index = (currentSelectedIndex + 1) % playerUnits.Count;
        bool isNextUnitFound = false;

        while (index != currentSelectedIndex && !isNextUnitFound) {
            if (playerUnits[index].HasRemainingActionPoints()) {
                isNextUnitFound = true;
            } else {
                index = (index + 1) % playerUnits.Count;
            }
        }

        return isNextUnitFound ? playerUnits[index] : null;
    }
}