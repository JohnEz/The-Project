using UnityEngine;
using UnityEngine.UI;

public class PregameUIController : MonoBehaviour {
    public Button startButton;

    public void Start() {
        startButton.interactable = false;
    }

    public void StartGame() {
        if (GameDetails.Party.Count <= 0) {
            return;
        }

        SaveSystem.Save();
    }

    public void Update() {
        if (!startButton.interactable && GameDetails.Party.Count > 0) {
            startButton.interactable = true;
        }
    }
}