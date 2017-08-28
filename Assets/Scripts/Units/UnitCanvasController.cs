using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UnitCanvasController : MonoBehaviour {

	public GameObject hpBarPrefab;
	public GameObject damageTextPrefab;

	HpBarController hpBar;

	Color[] teamColours = new Color[]{
		new Color(0,0,1),
		new Color(1,0,0),
		new Color(0,1,0)
	};

	// Use this for initialization
	void Start () {
		hpBar = Instantiate (hpBarPrefab).GetComponent<HpBarController>();
		hpBar.transform.SetParent (transform, false);
		//hpBar.
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
