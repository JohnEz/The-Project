using UnityEngine;
using System.Collections.Generic;

public class UserInterfaceManager : MonoBehaviour {
    public static UserInterfaceManager instance;

    //Managers
    private PauseMenuController pauseMenuController;

    private Node lastAttackedNode;

    private void Awake() {
        instance = this;
    }

    // Use this for initialization
    private void Start() {
        pauseMenuController = GetComponent<PauseMenuController>();
    }

    // Update is called once per frame
    private void Update() {
        UserControls();
    }

    public void ShowAbility(Ability ability) {
        UnshowAbility();

        if (!UnitSelectionManager.instance.IsAnAbilityActive()) {
            UnitSelectionManager.instance.ActiveAbility = ability;
            ShowAction();
        }
    }

    public void UnshowAbility() {
        ActionBar.instance.UnselectAbilities();
        HighlightManager.instance.UnhighlightTiles();
        HighlightManager.instance.ClearEffectedTiles();
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
                if (!ShowMovement()) {
                    UnshowAbility();
                    UnitSelectionManager.instance.UnselectUnit();
                }
            } else if (UnitSelectionManager.instance.IsDisplayingMovement()) {
                UnshowAbility();
                UnitSelectionManager.instance.UnselectUnit();
            } else if (PauseMenuController.gameIsPaused) {
                pauseMenuController.Resume();
            } else {
                pauseMenuController.Pause();
            }
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
                UnitSelectionManager.instance.SelectedUnit.ActionPoints -= UnitSelectionManager.instance.ActiveAbility.actionPointCost;
            }
        }
    }

    public void ClickedMovement(Node node) {
        UnitSelectionManager.instance.SelectedUnit.ActionPoints -= (int)node.moveCost;
        UnitManager.instance.MoveToTile(UnitSelectionManager.instance.SelectedUnit, node);
        UnshowAbility();
    }

    public void ClickedUnselected(Node node) {
        // TODO, use same flow as cancel
        UnshowAbility();

        if (!node.myUnit) {
            UnitSelectionManager.instance.UnselectUnit();
            return;
        }

        UnitSelectionManager.instance.SelectUnit(node.myUnit);
        UnitController selectedUnit = UnitSelectionManager.instance.SelectedUnit;

        if (TurnManager.instance.PlayersTurn == selectedUnit.myPlayer.id && selectedUnit.myStats.ActionPoints > 0) {
            ShowMovement();
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

    public bool ShowMovement() {
        if (!UnitSelectionManager.instance.CanDisplayMovement() || UnitSelectionManager.instance.SelectedUnit == null) {
            return false;
        }

        UnitController selectedUnit = UnitSelectionManager.instance.SelectedUnit;
        UnitManager.instance.ShowMoveAction(selectedUnit, selectedUnit.myStats.Speed, selectedUnit.myStats.WalkingType);
        UnitSelectionManager.instance.DisplayMovement();
        return true;
    }

    public bool UseAbility(int i) {
        if (!UnitSelectionManager.instance.SelectedUnit) {
            GUIController.instance.ShowErrorMessage("No unit Selected");
            return false;
        }

        Ability ability = UnitSelectionManager.instance.SelectedUnit.myStats.instantiatedAbilities[i];

        if (ability.IsOnCooldown()) {
            GUIController.instance.ShowErrorMessage("Ability is on cooldown");
            return false;
        }

        if (UnitSelectionManager.instance.SelectedUnit.myStats.ActionPoints < ability.actionPointCost) {
            GUIController.instance.ShowErrorMessage("Not enough action points");
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

    public void FinishedAction() {
        UnitSelectionManager.instance.CurrentActionIndex++;
        if (!RunNextAbilityAction(UnitSelectionManager.instance.ActiveAbility, UnitSelectionManager.instance.CurrentActionIndex)) {
            UnitSelectionManager.instance.FinishedUsingAbility();
        }
    }

    public void FinishedAttacking() {
        if (!TurnManager.instance.isAiTurn()) {
            FinishedAction();
        }
    }

    public void FinishedMoving() {
        if (!TurnManager.instance.isAiTurn()) {
            FinishedAction();
        }
    }
}