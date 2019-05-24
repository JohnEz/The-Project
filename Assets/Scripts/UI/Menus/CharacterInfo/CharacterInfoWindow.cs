using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

public class CharacterInfoWindow : MonoBehaviour {

    [Serializable] public class OnCharacterChangeEvent : UnityEvent<UnitObject> { }

    public OnCharacterChangeEvent onCharacterChange = new OnCharacterChangeEvent();

    private UnitObject character;

    public UnitObject Character {
        get { return character; }
        set {
            character = value;

            // Invoke the on assign event
            if (this.onCharacterChange != null)
                this.onCharacterChange.Invoke(character);
        }
    }
}