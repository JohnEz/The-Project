using TMPro;
using UnityEngine;

public class PlayMenuController : MonoBehaviour {
    public static Vector3 OPEN_POSITION = new Vector3(0, 0, 0);
    public static Vector3 SUBMENU_POSITON = new Vector3(515f, 3f, 0f);

    public GameObject subMenu;

    public AudioClip buttonClickAudio;

    public void PlayGameFireMage() {
        AudioManager.singleton.Play(buttonClickAudio, transform, AudioMixers.UI, true);

        GameDetails.PlayerCharacter = "Mage";
        GameDetails.PlayerDeck = BasicDecks.starterElementalist;
        UpdateSubmenu("Elementalist", "Description text for a Elementalist.");
        OpenSubMenu();
    }

    public void PlayGameElementalist() {
        AudioManager.singleton.Play(buttonClickAudio, transform, AudioMixers.UI, true);

        GameDetails.PlayerCharacter = "Warrior";
        GameDetails.PlayerDeck = BasicDecks.starterFighter;
        UpdateSubmenu("Warrior", "Description text for a Warrior.");
        OpenSubMenu();
    }

    private void UpdateSubmenu(string title, string description) {
        // set title
        subMenu.transform.Find("Title").gameObject.GetComponent<TextMeshProUGUI>().text = title;

        // set description
        subMenu.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = description;
    }

    public void OpenMenu() {
        GetComponent<SlidingMenu>().SlideToPosition(OPEN_POSITION);
    }

    public void CloseMenu() {
        GetComponent<SlidingMenu>().CloseMenu();
    }

    public void OpenSubMenu() {
        subMenu.GetComponent<SlidingMenu>().SlideToPosition(SUBMENU_POSITON);
    }

    public void CloseSubMenu() {
        subMenu.GetComponent<SlidingMenu>().CloseMenu();
    }

    public void PlayGame() {
        AudioManager.singleton.Play(buttonClickAudio, transform, AudioMixers.UI, true);

        LoadArena();
    }

    private void LoadArena() {
        SceneChanger.Instance.FadeToScene(Scenes.ARENA);
    }

    public void Back() {
        AudioManager.singleton.Play(buttonClickAudio, transform, AudioMixers.UI, true);

        transform.parent.Find("MainMenu").GetComponent<MainMenuController>().OpenMenu();
        CloseMenu();
        CloseSubMenu();
    }
}