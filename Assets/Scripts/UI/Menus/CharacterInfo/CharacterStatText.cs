using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using TMPro;

public enum UnitStatText {
    NAME,
    CLASS,
    SPEED,
}

public class CharacterStatText : MonoBehaviour {
    public UnitStatText watchedStat = UnitStatText.NAME;
    public bool displayAsPercentage = false;

    private UnitObject currentCharacter;
    private CharacterInfoWindow characterInfoWindow;

    private TextMeshProUGUI text;

    public void Awake() {
        characterInfoWindow = GetComponentInParent<CharacterInfoWindow>();
        text = GetComponent<TextMeshProUGUI>();
    }

    public void OnEnable() {
        if (characterInfoWindow) {
            characterInfoWindow.onCharacterChange.AddListener(OnCharacterUpdate);
        }

        if (currentCharacter != null) {
            currentCharacter.onStatChange.AddListener(OnStatUpdate);
        }
    }

    public void OnDisable() {
        if (characterInfoWindow) {
            characterInfoWindow.onCharacterChange.RemoveListener(OnCharacterUpdate);
        }

        if (currentCharacter != null) {
            currentCharacter.onStatChange.RemoveListener(OnStatUpdate);
        }
    }

    public void OnCharacterUpdate(UnitObject character) {
        if (character == currentCharacter) {
            return;
        }

        if (currentCharacter != null) {
            currentCharacter.onStatChange.RemoveListener(OnStatUpdate);
        }

        currentCharacter = character;

        if (currentCharacter != null) {
            currentCharacter.onStatChange.AddListener(OnStatUpdate);
        }

        OnStatUpdate();
    }

    public void OnStatUpdate() {
        text.text = GetStat(watchedStat) + (displayAsPercentage ? "%" : "");
    }

    public string GetStat(UnitStatText stat) {
        if (currentCharacter == null) {
            return "";
        }

        switch (stat) {
            case UnitStatText.NAME: return currentCharacter.characterName;
            case UnitStatText.CLASS: return currentCharacter.className;
            case UnitStatText.SPEED: return currentCharacter.Speed.ToString();
        }

        return "";
    }
}