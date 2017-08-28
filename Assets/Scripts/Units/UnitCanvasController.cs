using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UnitCanvasController : MonoBehaviour {

	public GameObject hpBarPrefab;
	public GameObject damageTextPrefab;

	HpBarController hpBar;

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
