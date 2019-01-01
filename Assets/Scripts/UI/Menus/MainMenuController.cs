using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

	public void Play () {
        transform.parent.Find("PlayMenu").gameObject.SetActive(true);
        gameObject.SetActive(false);
	}

	public void Exit () {
		Application.Quit ();
	}
}

