using UnityEngine;
using UnityEngine.UI;

public class PregameUIController : MonoBehaviour {
    public Button startButton;
    public PartyList partList;

    public void Start() {
        startButton.interactable = false;
    }

    public void OnEnable() {
        partList.onAdd.AddListener(UpdateButton);
        partList.onRemove.AddListener(UpdateButton);
    }

    public void OnDisable() {
        partList.onAdd.RemoveListener(UpdateButton);
        partList.onRemove.RemoveListener(UpdateButton);
    }

    public void StartGame() {
        if (GameDetails.Party.Count <= 0) {
            return;
        }

        SaveSystem.Save();
    }

    public void UpdateButton(int size) {
        if (startButton == null) {
            return;
        }

        startButton.interactable = size > 0;
    }
}