using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    private void Start() {
        LoadScene(SceneChanger.Instance.SceneToLoad);
    }

    public void LoadScene(int sceneIndex) {
        StartCoroutine(LoadAsync(sceneIndex));
    }

    private IEnumerator LoadAsync(int sceneIndex) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone) {
            //Load progress
            Debug.Log(operation.progress);

            yield return null;
        }
    }
}