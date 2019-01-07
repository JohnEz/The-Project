using UnityEngine;

public class PregameUIController : MonoBehaviour {

    public void EnterMatch() {
        SceneChanger.Instance.FadeToScene(Scenes.ARENA);
    }
}