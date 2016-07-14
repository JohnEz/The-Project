using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UserInterfaceController : MonoBehaviour {

	public GameObject turnTextPrefab;
	bool showingTurn = false;
	GameObject turnText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if (showingTurn) {
			if (turnText == null) {
				GetComponentInParent<TurnManager> ().FinishStartingTurn ();
				showingTurn = false;
			}
		}

	}

	public void StartNewTurn(string text) {
		showingTurn = true;
		turnText = (GameObject)Instantiate (turnTextPrefab, turnTextPrefab.transform.position, Quaternion.identity);
		turnText.transform.SetParent (transform);
		turnText.transform.localPosition = turnTextPrefab.transform.position;
		turnText.GetComponent<Text> ().text = text;

		//TODO THIS SHOULD FIND THE LENGTH OF THE ANIMATION AND NOT BE HARD CODED BUT IM TIRED
		Destroy (turnText, 2);

	}
}
