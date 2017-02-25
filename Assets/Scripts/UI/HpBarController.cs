﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBarController : MonoBehaviour {

	public Image hpBar;

	// Use this for initialization
	void Start () {
		hpBar = transform.FindChild("hpBar").GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetHP(float percentage) {
		hpBar.fillAmount = percentage;
	}
}