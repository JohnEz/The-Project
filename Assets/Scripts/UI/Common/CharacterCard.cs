using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using DuloGames.UI;

public class CharacterCard : MonoBehaviour {
    private UnitObject myCharacter;

    public Image characterImage;

    public void Start() {
        UpdateDisplay();
    }

    public UnitObject MyCharacter {
        get {
            return myCharacter;
        }
        set {
            SetMyCharacter(value);
        }
    }

    private void UpdateDisplay() {
        characterImage.sprite = myCharacter.unitTokens[0].frontSprite;
    }

    private void SetMyCharacter(UnitObject newCharacter) {
        myCharacter = newCharacter;
        UpdateDisplay();
    }

    public void OnClickInfo() {
        CharacterInfoWindow characterInfoWindow = UIWindow.GetWindow(UIWindowID.Character).GetComponent<CharacterInfoWindow>();
        if (characterInfoWindow == null) {
            return;
        }

        characterInfoWindow.Character = myCharacter;
    }
}