using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UnitCanvasController : MonoBehaviour {

	public GameObject hpBarPrefab;
	public GameObject damageTextPrefab;
	public GameObject actionPointPrefab;

	HpBarController hpBar;
	Stack<GameObject> actionPoints = new Stack<GameObject>();

	int myTeam;

	Color[] teamColours = new Color[]{
		new Color (0, 0.9647f, 1), //blue
		new Color (0.8431f, 0.2f, 0.2f), //red
		new Color (0.7294f, 0.9569f, 0.1176f) //green
	};

	// Use this for initialization
	void Start () {
		myTeam = GetComponentInParent<UnitController> ().myPlayer.id;
		hpBar = Instantiate (hpBarPrefab).GetComponent<HpBarController>();
		hpBar.transform.SetParent (transform, false);
		hpBar.Initialize ();
		hpBar.SetHPColor (teamColours [myTeam]);
    }

	public void SetActionPoints (int newActionPoints) {
		if (newActionPoints > actionPoints.Count) {
			int difference = newActionPoints - actionPoints.Count;
			for (int i = 0; i < difference; i++) {
				AddActionPoint ();
			}
		} else if (newActionPoints < actionPoints.Count) {
			int difference = actionPoints.Count - newActionPoints;
			for (int i = 0; i < difference; i++) {
				RemoveActionPoint ();
			}
		}
	}

	void AddActionPoint() {
		GameObject newActionPoint = Instantiate (actionPointPrefab);
		Vector3 newPosition = newActionPoint.transform.position;
		newPosition.x = -22 + (actionPoints.Count * 33);
		newActionPoint.transform.position = newPosition;
		newActionPoint.transform.SetParent (transform, false);
		actionPoints.Push(newActionPoint);
	}

	void RemoveActionPoint() {
		if (actionPoints.Count > 0) {
			GameObject removeObject = actionPoints.Pop ();
			Destroy (removeObject);
		}
	}

	// Update is called once per frame
	void Update () {

	}

	public void UpdateHP(float currentHP, float maxHP) {
		hpBar.SetHP(currentHP / maxHP);
	}

	public void CreateDamageText(int damage) {
		GameObject newDamageText = Instantiate (damageTextPrefab);
		newDamageText.GetComponent<Text> ().text = damage.ToString();
		newDamageText.transform.SetParent(this.transform);
	}

}
