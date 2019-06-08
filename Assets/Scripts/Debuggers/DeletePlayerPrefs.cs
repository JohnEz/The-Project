using UnityEngine;

public class DeletePlayerPrefs : MonoBehaviour {

    private void OnGUI() {
        if (Debug.isDebugBuild) {
            //Delete all of the PlayerPrefs settings by pressing this Button
            if (GUI.Button(new Rect(0, 0, 200, 60), "Delete Player Prefs")) {
                PlayerPrefs.DeleteAll();
            }
        }
    }
}