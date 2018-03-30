using UnityEngine.SceneManagement;

public static class MenuSystem {
	private static int sceneToLoad;

	public static int SceneToLoad {
		get {
			return sceneToLoad;
		}
		set {
			sceneToLoad = value;
		}
	}

	public static void LoadScene(int scene) {
		sceneToLoad = scene;
		SceneManager.LoadScene (Scenes.LOADING);
	}
}