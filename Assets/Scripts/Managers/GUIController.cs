﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GUIController : MonoBehaviour {

    public static GUIController singleton;

	public GameObject turnTextPrefab;
	bool showingTurn = false;
	GameObject turnText;

	public GameObject errorMessagePrefab;

	List<GameObject> abilityIcons = new List<GameObject>();
	Dictionary<string, RuntimeAnimatorController> abilityIconControllers = new Dictionary<string, RuntimeAnimatorController>();

    public Text staminaText;

	Text objectivesBody;

	void Awake() {
        singleton = this;
        staminaText = GameObject.Find("ActionPointFrame").GetComponentInChildren<Text>();
	}

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		//TODO this should probably be some sort of timer so there is less of a chance the user can get stuck
		if (showingTurn) {
			if (turnText == null) {
				TurnManager.singleton.FinishStartingTurn ();
				showingTurn = false;
			}
		}
	}

	public void StartNewTurn(bool ally, List<Objective> objectives) {
		showingTurn = true;
		turnText = (GameObject)Instantiate (turnTextPrefab, turnTextPrefab.transform.position, Quaternion.identity);
		turnText.transform.SetParent (transform);
		turnText.transform.localPosition = turnTextPrefab.transform.position;
		turnText.transform.localScale = new Vector3 (1, 1, 1);
		turnText.transform.Find("AllyTurnImage").gameObject.SetActive(ally);
		turnText.transform.Find("EnemyTurnImage").gameObject.SetActive(!ally);

		if (objectivesBody == null) {
			objectivesBody = transform.Find ("ObjectivesBody").GetComponent<Text> ();
		}
		objectivesBody.text = CreateObjectiveText (objectives);

		//TODO THIS SHOULD FIND THE LENGTH OF THE ANIMATION AND NOT BE HARD CODED BUT IM TIRED
		Destroy (turnText, 1.917f);

	}

	public void ClearAbilityIcons() {
		abilityIcons.ForEach ((icon) => {
			Destroy (icon.gameObject);
			AbilityDeselected(abilityIcons.IndexOf(icon));
		});
		abilityIcons.Clear ();
	}

	public void AbilitySelected(int abilityIndex) {
		if (abilityIcons.Count > abilityIndex) {
			abilityIcons [abilityIndex].GetComponent<Animator> ().SetBool("active", true);
		}
	}

	public void AbilityDeselected(int abilityIndex) {
		if (abilityIcons.Count > abilityIndex) {
			abilityIcons [abilityIndex].GetComponent<Animator> ().SetBool("active", false);
		}
	}

	RuntimeAnimatorController LoadRuntimeAnimatorController(string controller) {
		if (!abilityIconControllers.ContainsKey(controller)) {
			abilityIconControllers.Add (controller, Resources.Load<RuntimeAnimatorController> ("Graphics/UI/InGame/Icons/" + controller));
		}

		return abilityIconControllers [controller];
	}

    public void UpdateStamina(int newStamina) {
        staminaText.text = newStamina.ToString();
    }

	string CreateObjectiveText(List<Objective> objectives) {
		string constructedObjectiveText = "";
		foreach (Objective objective in objectives) {
			constructedObjectiveText += objective.text + "\n";
		}
		return constructedObjectiveText;
	}

	public void ShowErrorMessage(string message) {
		GameObject newDamageText = Instantiate (errorMessagePrefab);
		newDamageText.GetComponent<Text> ().text = message;
		newDamageText.GetComponent<Text> ().color = new Color(0.95f, 0.25f, 0.25f);
		newDamageText.transform.SetParent(this.transform);
	}
}
