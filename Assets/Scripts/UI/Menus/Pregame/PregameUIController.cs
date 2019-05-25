using UnityEngine;

public class PregameUIController : MonoBehaviour {

    public void StartGame() {
        if (GameDetails.Party.Count <= 0) {
            return;
        }

        SaveSystem.Save();
        SceneChanger.Instance.FadeToScene(Scenes.BATTLE);
    }
}