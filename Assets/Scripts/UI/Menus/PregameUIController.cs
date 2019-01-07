using UnityEngine;
using System.Collections;

public class PregameUIController : MonoBehaviour {

    public void EnterMatch() {
        SceneChanger.Instance.FadeToScene(Scenes.ARENA);
    }
}
