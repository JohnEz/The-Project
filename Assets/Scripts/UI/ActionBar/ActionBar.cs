using DuloGames.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionBar : MonoBehaviour {
    public static ActionBar instance;

    private List<UISpellSlot> slots;

    [HideInInspector]
    public UnitController currentlyDisplayedUnit;

    public Image avatarImage;

    public BuffController buffController;

    public void Awake() {
        instance = this;
    }

    public void Start() {
        slots = UISpellSlot.GetSlotsInGroup(UISpellSlot_Group.Main_1);
    }

    public void Update() {
        // TODO change this to use an event listener
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
        UpdateSlots();
        UpdateAvatar();
        UpdateBuffController();
    }

    public void UpdateSlots() {
        int index = 0;
        slots.ForEach((slot) => {
            if (!currentlyDisplayedUnit || currentlyDisplayedUnit.myStats.instantiatedAbilities.Count <= index) {
                slot.Unassign();
                return;
            }
            Ability abilityToDisplay = currentlyDisplayedUnit.myStats.instantiatedAbilities[index];
            slot.Assign(abilityToDisplay.ToAbilityInfo());

            index++;
        });
    }

    public void UpdateAvatar() {
        if (currentlyDisplayedUnit == null) {
            avatarImage.sprite = null;
            avatarImage.enabled = false;
            return;
        }

        avatarImage.enabled = true;
        Sprite avatar = currentlyDisplayedUnit.myStats.displayToken.frontSprite;
        avatarImage.sprite = avatar;
        avatarImage.rectTransform.sizeDelta = avatar.rect.size;

        Vector3 currentImagePosition = avatarImage.rectTransform.anchoredPosition;
        avatarImage.rectTransform.anchoredPosition = new Vector3(currentImagePosition.x, -(avatar.rect.height / 4), currentImagePosition.z);
    }

    public void UpdateBuffController() {
        if (buffController == null) {
            return;
        }

        UnitBuffs buffs = currentlyDisplayedUnit != null ? currentlyDisplayedUnit.myStats.buffs : null;
        buffController.Initialise(buffs);
    }
}