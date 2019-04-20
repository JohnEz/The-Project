using UnityEngine;

public class PregameUIController : MonoBehaviour {

    public void StartGame() {
        if (GameDetails.Party.Count <= 0) {
            return;
        }

        SceneChanger.Instance.FadeToScene(Scenes.BATTLE);
    }
}