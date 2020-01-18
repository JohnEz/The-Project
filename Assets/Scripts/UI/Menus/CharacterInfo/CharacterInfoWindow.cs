using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using DuloGames.UI;

public class CharacterInfoWindow : MonoBehaviour {

    [Serializable] public class OnCharacterChangeEvent : UnityEvent<UnitObject> { }

    public OnCharacterChangeEvent onCharacterChange = new OnCharacterChangeEvent();

    private UnitObject character;

    private List<UISpellSlot> spellSlots;
    private List<EquipmentSlot> equiptmentSlots;

    public void Start() {
        spellSlots = UISpellSlot.GetSlotsInGroup(UISpellSlot_Group.SpellBook);
        spellSlots.Sort((l, r) => l.ID > r.ID ? 1 : -1);

        equiptmentSlots = EquipmentSlot.GetSlots();
    }

    public UnitObject Character {
        get { return character; }
        set {
            character = value;

            // Invoke the on assign event
            if (this.onCharacterChange != null)
                this.onCharacterChange.Invoke(character);

            SetAbilitySlots();
            SetEquiptment();
        }
    }

    public void SetAbilitySlots() {
        int index = 0;
        spellSlots.ForEach((slot) => {
            if (Character == null || Character.instantiatedAbilities.Count <= index) {
                slot.Unassign();
                return;
            }
            Ability abilityToDisplay = Character.instantiatedAbilities[index];
            slot.Assign(abilityToDisplay.ToAbilityInfo());

            index++;
        });
    }

    public void SetEquiptment() {
        equiptmentSlots.ForEach(slot => {
            if (Character == null) {
                slot.Unassign();
                return;
            }

            ItemInfo itemInSlot = Character.equipment.GetItemInSlot(slot.EquipSlotType);

            if (itemInSlot != null) {
                slot.Assign(itemInSlot);
            } else {
                slot.Unassign();
            }
        });
    }
}