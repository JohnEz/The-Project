using UnityEngine;
using System.Collections;

public class PregameUIController : MonoBehaviour {

    public void EnterMatch() {
        MenuSystem.LoadScene(Scenes.ARENA);
    }
}
