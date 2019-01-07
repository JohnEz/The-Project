using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour {

    public static SceneChanger Instance;

    public Animator animator;

    private static int sceneToLoad;

    private bool changingScene = false;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (Instance != this) {
            Destroy(this.gameObject);
            return;
        }
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Use this for initialization
    void Start() {

    }

    public int SceneToLoad {
        get {
            return sceneToLoad;
        }
        set {
            sceneToLoad = value;
        }
    }

    // Update is called once per frame
    void Update() {

    }

    public void FadeToScene(int sceneIndex) {
        sceneToLoad = sceneIndex;
        changingScene = true;
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete () {
        SceneManager.LoadScene(Scenes.LOADING);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (changingScene) {
            changingScene = false;
            animator.SetTrigger("FadeIn");
        }
    }
}
