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

    private List<UISpellSlot> slots;

    public void Start() {
        slots = UISpellSlot.GetSlotsInGroup(UISpellSlot_Group.SpellBook);
        slots.Sort((l, r) => l.ID > r.ID ? 1 : -1);
    }

    public UnitObject Character {
        get { return character; }
        set {
            character = value;

            // Invoke the on assign event
            if (this.onCharacterChange != null)
                this.onCharacterChange.Invoke(character);

            SetAbilitySlots();
        }
    }

    public void SetAbilitySlots() {
        int index = 0;
        slots.ForEach((slot) => {
            if (!Character || Character.instantiatedAbilities.Count <= index) {
                slot.Unassign();
                return;
            }
            Ability abilityToDisplay = Character.instantiatedAbilities[index];
            slot.Assign(abilityToDisplay.ToAbilityInfo());

            index++;
        });
    }
}