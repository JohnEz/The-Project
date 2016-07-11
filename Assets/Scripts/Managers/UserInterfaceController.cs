using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UserInterfaceController : MonoBehaviour {

	public GameObject turnTextPrefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool StartNewTurn(string text) {
		GameObject turnText = (GameObject)Instantiate (turnTextPrefab);
		turnText.GetComponent<Text> ().text = text;

		Animator animator = turnText.GetComponent<Animator> ();

		return true;

		//animator.anima.PlayQueued( "Something" );
	}

	private IEnumerator WaitForAnimation ( Animation animation )
	{
		do
		{
			yield return null;
		} while ( animation.isPlaying );
	}
}
