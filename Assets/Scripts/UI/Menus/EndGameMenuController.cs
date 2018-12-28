using UnityEngine;
using System.Collections;

public class EndGameMenuController : MonoBehaviour {

    public void Quit() {
        MenuSystem.LoadScene(Scenes.MAIN_MENU);
    }

    public void Retry() {
        MenuSystem.LoadScene(Scenes.ARENA);
    }
}
