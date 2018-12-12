using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

	void Start () {
		LoadScene (MenuSystem.SceneToLoad);
	}

	public void LoadScene (int sceneIndex) {
		StartCoroutine (LoadAsync (sceneIndex));
	}

	IEnumerator LoadAsync (int sceneIndex) {
		AsyncOperation operation = SceneManager.LoadSceneAsync (sceneIndex);

		while (!operation.isDone) {
            //Load progress
			Debug.Log (operation.progress);

			yield return null;
		}
	}
}

