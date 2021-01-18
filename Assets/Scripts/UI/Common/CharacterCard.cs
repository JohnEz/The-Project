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
        UIWindow characterWindow = UIWindow.GetWindow(UIWindowID.Character);
        CharacterInfoWindow characterInfoWindow = characterWindow.GetComponent<CharacterInfoWindow>();
        if (characterInfoWindow == null) {
            return;
        }

        characterWindow.Show();
        characterInfoWindow.Character = myCharacter;
    }
}