using UnityEngine;
using System.Collections.Generic;

public class UserInterfaceManager : MonoBehaviour {
    public static UserInterfaceManager instance;

    private Node lastAttackedNode;

    private void Awake() {
        instance = this;
    }

    // Update is called once per frame
    private void Update() {
        UserControls();
    }

    public void ShowAbilityBySlot(int slot) {
        UseAbility(slot);
    }

    public void ShowAbility(Ability ability) {
        UnshowAbility();

        if (!UnitSelectionManager.instance.IsAnAbilityActive()) {
            UnitSelectionManager.instance.ActiveAbility = ability;
            ShowAction();
        }
    }

    public void UnshowAbility(bool clearSelectedUnit = false) {
        ActionBar.instance.UnselectAbilities();
        HighlightManager.instance.UnhighlightNodes();
        HighlightManager.instance.ClearEffectedNodes(clearSelectedUnit);
    }

    private void UserControls() {
        //temp for ai
        //if (Input.GetKeyUp ("space")) {
        //	TurnManager.instance.EndTurn ();
        //}

        //Cancel (right click)
        if (Input.GetKeyUp(KeyCode.Escape) || Input.GetMouseButtonUp(1)) {
            // If the player is using an ability and has done atleast 1 action
            if (UnitSelectionManager.instance.IsAnAbilityInprogress()) {
                //check to see if they have an available option
                if (ActionHasAvailableOptions()) {
                    GUIController.instance.ShowErrorMessage("You have available options!");
                } else {
                    UnshowAbility();
                    UnitSelectionManager.instance.FinishedUsingAbility();
                }
            } else if (UnitSelectionManager.instance.IsAnAbilitySelected()) {
                UnitSelectionManager.instance.CancelCurrentAbility();
                if (!ShowAvailableAction()) {
                    UnshowAbility();
                    UnitSelectionManager.instance.UnselectUnit();
                }
            } else if (UnitSelectionManager.instance.IsDisplayingMovement()) {
                UnshowAbility();
                CancelMovement();
                UnitSelectionManager.instance.UnselectUnit();
            } else if (Input.GetKeyUp(KeyCode.Escape)) {
                if (PauseMenuController.gameIsPaused) {
                    PauseMenuController.instance.Resume();
                } else {
                    PauseMenuController.instance.Pause();
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.Tab)) {
            SelectNextUnit();
        }

        if (UnitSelectionManager.instance.SelectedUnit) {
            List<Ability> abilities = UnitSelectionManager.instance.SelectedUnit.myStats.instantiatedAbilities;

            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                if (abilities.Count < 1) {
                    return;
                }
                UseAbility(0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                if (abilities.Count < 2) {
                    return;
                }
                UseAbility(1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3)) {
                if (abilities.Count < 3) {
                    return;
                }
                UseAbility(2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4)) {
                if (abilities.Count < 4) {
                    return;
                }
                UseAbility(3);
            }
        }

        if (Input.GetKeyUp("space")) {
            EndTurn();
        }
    }

    public void EndTurn() {
        if (TurnManager.instance.CurrentPhase == TurnPhase.WAITING_FOR_INPUT && !TurnManager.instance.isAiTurn()) {
            UnshowAbility();
            if (UnitSelectionManager.instance.IsAnAbilityActive()) {
                UnitSelectionManager.instance.CancelCurrentAbility();
            }
            UnitSelectionManager.instance.UnselectUnit();
            TurnManager.instance.EndTurn();
        }
    }

    public void TileHovered(Node node, SquareTarget target) {
        UnitManager.instance.CurrentlyHoveredNode = node;
        if (UnitSelectionManager.instance.IsAnAbilityActive() && (target == SquareTarget.ATTACK || target == SquareTarget.HELPFULL)) {
            UnitManager.instance.HighlightEffectedTiles(UnitSelectionManager.instance.ActiveAbility.caster, node);
        } else if (target == SquareTarget.MOVEMENT || target == SquareTarget.DASH || ((target == SquareTarget.ATTACK || target == SquareTarget.HELPFULL) && node.previousMoveNode != null)) {
            UnitManager.instance.ShowPath(node);
        }
    }

    public void TileExit(Node node, SquareTarget target) {
        UnitManager.instance.CurrentlyHoveredNode = null;
        UnitManager.instance.UnhiglightEffectedTiles();
    }

    public void TileClicked(Node node, SquareTarget target) {
        if (TurnManager.instance.CurrentPhase == TurnPhase.WAITING_FOR_INPUT && !TurnManager.instance.isAiTurn()) {
            switch (target) {
                case SquareTarget.HELPFULL:
                case SquareTarget.ATTACK:
                    ClickedAttack(node);
                    break;

                case SquareTarget.DASH:
                case SquareTarget.MOVEMENT:
                    ClickedMovement(node);
                    break;

                case SquareTarget.NONE:
                default:
                    ClickedUnselected(node);
                    break;
            }
        }
    }

    public void ClickedAttack(Node node) {
        if (UnitManager.instance.AttackTile(UnitSelectionManager.instance.ActiveAbility.caster, node)) {
            UnshowAbility();
            UnitSelectionManager.instance.AbilityInprogress();
            lastAttackedNode = node;

            if (UnitSelectionManager.instance.CurrentActionIndex == 0) {
                UnitSelectionManager.instance.SelectedUnit.myStats.ActionPoints -= UnitSelectionManager.instance.ActiveAbility.actionPointCost;
            }
        }
    }

    public void ClickedMovement(Node node) {
        UnshowAbility(true);

        UnitController selectedUnit = UnitSelectionManager.instance.SelectedUnit;

        // TODO this isnt always the right tile, need to find the closest
        Tile tile = TileMap.instance.GetTileClosestToClick(selectedUnit.myStats.size, selectedUnit.myTile.x, selectedUnit.myTile.y, node.x, node.y);

        int cost = Mathf.CeilToInt(tile.cost / selectedUnit.myStats.Speed);

        UnitSelectionManager.instance.SelectedUnit.myStats.MoveActionPoints -= cost;
        UnitManager.instance.MoveToTile(selectedUnit, tile);
    }

    public void ClickedUnselected(Node node) {
        // TODO, use same flow as cancel
        UnshowAbility();

        if (!node.MyUnit) {
            UnitSelectionManager.instance.UnselectUnit();
            return;
        }

        UnitSelectionManager.instance.SelectUnit(node.MyUnit);
        UnitController selectedUnit = UnitSelectionManager.instance.SelectedUnit;

        if (TurnManager.instance.PlayersTurn == selectedUnit.myPlayer.id) {
            ShowAvailableAction();
        }
    }

    public void AbilityHovered(Ability ability) {
        if (UnitSelectionManager.instance.CanUseAbility()) {
            ShowAbility(ability);
        }
    }

    public void AbilityUnhovered() {
        // if there isnt a played card, clear the display
        if (!UnitSelectionManager.instance.IsAnAbilityActive()) {
            UnshowAbility();
            UnitSelectionManager.instance.ActiveAbility = null;
        }
    }

    public bool ShowAvailableAction() {
        UnitController selectedUnit = UnitSelectionManager.instance.SelectedUnit;

        if (selectedUnit == null) {
            return false;
        }

        UnshowAbility();

        if (selectedUnit.myStats.MoveActionPoints > 0) {
            ShowMovement();
            return true;
        }

        if (selectedUnit.myStats.ActionPoints > 0) {
            UseAbility(0);
            return true;
        }

        return false;
    }

    public bool ShowMovement() {
        if (!UnitSelectionManager.instance.CanDisplayMovement() || UnitSelectionManager.instance.SelectedUnit == null || UnitSelectionManager.instance.SelectedUnit.myStats.MoveActionPoints < 1) {
            return false;
        }

        UnitController selectedUnit = UnitSelectionManager.instance.SelectedUnit;
        UnitManager.instance.ShowMoveAction(selectedUnit, selectedUnit.myStats.Speed, selectedUnit.myStats.WalkingType);
        UnitSelectionManager.instance.DisplayMovement();
        return true;
    }

    public void CancelMovement() {
        if (!UnitSelectionManager.instance.IsDisplayingMovement()) {
            return;
        }

        UnitSelectionManager.instance.ResetState();
    }

    public bool UseAbility(int i) {
        if (!UnitSelectionManager.instance.SelectedUnit) {
            GUIController.instance.ShowErrorMessage("No unit Selected");
            return false;
        }

        Ability ability = UnitSelectionManager.instance.SelectedUnit.myStats.instantiatedAbilities[i];

        if (UnitSelectionManager.instance.SelectedUnit.myStats.ActionPoints < ability.actionPointCost) {
            GUIController.instance.ShowErrorMessage("Not enough action points");
            return false;
        }

        if (ability.IsOnCooldown()) {
            GUIController.instance.ShowErrorMessage("Ability is on cooldown");
            return false;
        }

        if (!ability.HasRemainingUses()) {
            GUIController.instance.ShowErrorMessage("No uses left");
            return false;
        }

        if (UnitSelectionManager.instance.CanUseAbility(ability)) {
            UnshowAbility();
            ActionBar.instance.SelectAbility(i);
            UnitSelectionManager.instance.UseAbility(ability);
            return true;
        }
        return false;
    }

    public bool RunNextAbilityAction(Ability ability, int actionIndex) {
        if (ability && actionIndex < ability.Actions.Count) {
            AbilityAction currentAction = ability.Actions[actionIndex];
            if (currentAction.GetType() == typeof(MoveAction) || typeof(AttackAction).IsAssignableFrom(currentAction.GetType())) {
                ShowAction();

                if (typeof(AttackAction).IsAssignableFrom(currentAction.GetType()) && ((AttackAction)currentAction).isAutoCast) {
                    ClickedAttack(lastAttackedNode);
                }
            }

            return true;
        }

        return false;
    }

    public void ShowAction() {
        Ability activeAbility = UnitSelectionManager.instance.ActiveAbility;
        AbilityAction currentAction = activeAbility.Actions[UnitSelectionManager.instance.CurrentActionIndex];
        if (currentAction.GetType() == typeof(MoveAction)) {
            MoveAction moveAction = (MoveAction)currentAction;
            UnitManager.instance.ShowMoveAction(activeAbility.caster, moveAction.distance, moveAction.walkingType);
        } else if (typeof(AttackAction).IsAssignableFrom(currentAction.GetType())) {
            AttackAction attackAction = (AttackAction)currentAction;
            UnitManager.instance.ShowAttackAction(activeAbility.caster, attackAction);
        }
    }

    // if there is another action to the ability, display it
    public bool ActionHasAvailableOptions() {
        //CardSlot activeCard = CardManager.instance.ActiveCard;
        //AbilityAction currentAction = activeCard.card.Actions[CardManager.instance.CurrentActionIndex];
        //UnitController unit = activeCard.card.caster;
        //if (currentAction.GetType() == typeof(MoveAction)) {
        //    MoveAction moveAction = (MoveAction)currentAction;
        //    ReachableTiles walkingTiles = TileMap.instance.pathfinder.findReachableTiles(unit.myNode, moveAction.distance, moveAction.walkingType, unit.myPlayer.faction);
        //    return walkingTiles.basic.Keys.Count > 0;
        //} else if (typeof(AttackAction).IsAssignableFrom(currentAction.GetType())) {
        //    AttackAction attackAction = (AttackAction)currentAction;
        //    //UnitManager.instance.ShowAttackAction(activeCard.card.caster, attackAction);
        //    List<Node> attackableTiles = TileMap.instance.pathfinder.FindAttackableTiles(unit.myNode, attackAction);
        //    return attackableTiles.Exists(tile => attackAction.CanHitUnit(tile));
        //}

        return false;
    }

    public bool ReselectUnit() {
        UnitController lastSelectedCharacter = UnitSelectionManager.instance.selectedUnit;
        if (lastSelectedCharacter != null) {
            if (lastSelectedCharacter.HasRemainingActionPoints()) {
                UnitSelectionManager.instance.SelectUnit(lastSelectedCharacter);
                ShowAvailableAction();
                return true;
            }
        }
        return false;
    }

    public void SelectNextUnit() {
        UnitController nextUnit = UnitSelectionManager.instance.SelectNextUnit();
        if (nextUnit != null) {
            CameraManager.instance.JumpToLocation(nextUnit.myTile);
            ShowAvailableAction();
        }
    }

    //public void SelectPreviousUnit() {
    //    UnitController nextUnit = unitManager.GetPreviousUnit(turnManager.PlayersTurn);
    //    if (nextUnit != null) {
    //        GetComponent<CameraManager>().MoveToLocation(nextUnit.myNode);
    //        SelectUnit(nextUnit);
    //        ShowMovement();
    //    }
    //}

    public void FinishedAction() {
        UnitSelectionManager.instance.CurrentActionIndex++;
        if (!RunNextAbilityAction(UnitSelectionManager.instance.ActiveAbility, UnitSelectionManager.instance.CurrentActionIndex)) {
            UnitSelectionManager.instance.FinishedUsingAbility();

            if (!ReselectUnit()) {
                SelectNextUnit();
            }
        }
    }

    public void FinishedAttacking() {
        if (!TurnManager.instance.isAiTurn()) {
            // TODO i dont like this being in here also if a move has multiple attacks, this could be wrong?
            // Active ability can be null if it was a parry attack
            if (UnitSelectionManager.instance.ActiveAbility) {
                UnitSelectionManager.instance.ActiveAbility.SetOnCooldown(true);
                UnitSelectionManager.instance.ActiveAbility.RemainingUses -= 1;
            }

            FinishedAction();
        }
    }

    public void FinishedMoving() {
        if (!TurnManager.instance.isAiTurn()) {
            FinishedAction();
        }
    }
}