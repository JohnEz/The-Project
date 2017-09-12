using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIController : MonoBehaviour {

	public GameObject turnTextPrefab;
	bool showingTurn = false;
	GameObject turnText;

	TurnManager turnManager;


	// Use this for initialization
	void Start () {
		turnManager = GetComponentInParent<TurnManager> ();
	}

	// Update is called once per frame
	void Update () {
		//TODO this should probably be some sort of timer so there is less a chance the user can get stuck
		if (showingTurn) {
			if (turnText == null) {
				turnManager.FinishStartingTurn ();
				showingTurn = false;
			}
		}
	}

	public void StartNewTurn(bool ally) {
		showingTurn = true;
		turnText = (GameObject)Instantiate (turnTextPrefab, turnTextPrefab.transform.position, Quaternion.identity);
		turnText.transform.SetParent (transform);
		turnText.transform.localPosition = turnTextPrefab.transform.position;
		turnText.transform.localScale = new Vector3 (1, 1, 1);
		turnText.transform.FindChild("AllyTurnImage").gameObject.SetActive(ally);
		turnText.transform.FindChild("EnemyTurnImage").gameObject.SetActive(!ally);

		//TODO THIS SHOULD FIND THE LENGTH OF THE ANIMATION AND NOT BE HARD CODED BUT IM TIRED
		Destroy (turnText, 1.917f);

	}
}
