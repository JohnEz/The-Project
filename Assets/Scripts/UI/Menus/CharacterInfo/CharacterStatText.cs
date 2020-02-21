using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using TMPro;

public enum UnitStatText {
    NAME,
    CLASS,
    STRENGTH,
    AGILITY,
    CONSTITUTION,
    WISDOM,
    INTELLIGENCE,
    AC,
    HEALTH,
    SHIELD,
    SPEED,
    HIT,
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
            case UnitStatText.STRENGTH: return currentCharacter.Strength.ToString();
            case UnitStatText.AGILITY: return currentCharacter.Agility.ToString();
            case UnitStatText.CONSTITUTION: return currentCharacter.Constitution.ToString();
            case UnitStatText.WISDOM: return currentCharacter.Wisdom.ToString();
            case UnitStatText.INTELLIGENCE: return currentCharacter.Intelligence.ToString();
            case UnitStatText.AC: return currentCharacter.AC.ToString();
            case UnitStatText.HEALTH: return currentCharacter.Health.ToString() + " / " + currentCharacter.MaxHealth;
            case UnitStatText.SHIELD: return currentCharacter.Shield.ToString();
            case UnitStatText.SPEED: return currentCharacter.Speed.ToString();
            case UnitStatText.HIT: return currentCharacter.Hit.ToString();
        }

        return "";
    }
}