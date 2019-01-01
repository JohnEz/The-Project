using UnityEngine;
using System.Collections;
using TMPro;

public class PlayMenuController : MonoBehaviour {

    public static Vector3 SUBMENU_POSITON = new Vector3(408f, 10f, 0f);

    public GameObject subMenu;

    public void PlayGameFireMage() {
        GameDetails.PlayerCharacter = 1;
        GameDetails.PlayerDeck = BasicDecks.fireElementalist;
        UpdateSubmenu("Fire Mage", "Description text for a Fire Mage.");
        OpenSubMenu();
    }

    public void PlayGameElementalist() {
        GameDetails.PlayerCharacter = 1;
        GameDetails.PlayerDeck = BasicDecks.elementalistDeck;
        UpdateSubmenu("Elementalist", "Description text for a Elementalist.");
        OpenSubMenu();
    }

    private void UpdateSubmenu(string title, string description) {
        // set title
        subMenu.transform.Find("Header").gameObject.GetComponentInChildren<TextMeshProUGUI>().text = title;

        // set description
        subMenu.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = description;
    }

    public void OpenSubMenu() {
        subMenu.GetComponent<SlidingMenu>().SlideToPosition(SUBMENU_POSITON);
    }

    public void CloseSubMenu() {
        subMenu.GetComponent<SlidingMenu>().CloseMenu();
    }

    public void PlayGame() {
        LoadArena();
    }

    void LoadArena() {
        MenuSystem.LoadScene(Scenes.ARENA);
    }

    public void Back() {
        transform.parent.Find("MainMenu").gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
