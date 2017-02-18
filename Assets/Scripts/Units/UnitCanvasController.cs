using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UnitCanvasController : MonoBehaviour {

	public Image hpBar;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void UpdateHP(float currentHP, float maxHP) {
		hpBar.fillAmount = currentHP / maxHP;
	}
}
