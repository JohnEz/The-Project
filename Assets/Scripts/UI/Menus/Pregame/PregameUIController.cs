using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PregameUIController : MonoBehaviour {
    public Button startButton;
    public PartyList partList;
    public TextMeshProUGUI descriptionText;

    public void Start() {
        startButton.interactable = false;
        UpdateLevelDescription();
    }

    public void OnEnable() {
        partList.onAdd.AddListener(OnPartyChange);
        partList.onRemove.AddListener(OnPartyChange);
        GameDetails.onLevelChange.AddListener(OnLevelChange);
    }

    public void OnDisable() {
        partList.onAdd.RemoveListener(OnPartyChange);
        partList.onRemove.RemoveListener(OnPartyChange);
        GameDetails.onLevelChange.RemoveListener(OnLevelChange);
    }

    public void StartGame() {
        if (GameDetails.Party.Count <= 0) {
            return;
        }

        SaveSystem.Save();
    }

    public void OnPartyChange(int size) {
        UpdateButton();
    }

    public void OnLevelChange(LevelObject level) {
        UpdateButton();
        UpdateLevelDescription();
    }

    public void UpdateButton() {
        if (startButton == null) {
            return;
        }

        startButton.interactable = GameDetails.Party.Count > 0 && GameDetails.Party.Count <= GameDetails.MaxPartySize;
    }

    public void UpdateLevelDescription() {
        descriptionText.text = GameDetails.Level.description;
    }
}