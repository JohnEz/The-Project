using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UnitCanvasController : MonoBehaviour {

	public GameObject hpBarPrefab;
	public GameObject damageTextPrefab;

	HpBarController hpBar;

	int hpBarHeight = 100;


	// Use this for initialization
	void Start () {
		hpBar = Instantiate (hpBarPrefab).GetComponent<HpBarController>();
		hpBar.transform.SetParent (transform, false);
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
