using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GUIController : MonoBehaviour {

	public GameObject turnTextPrefab;
	bool showingTurn = false;
	GameObject turnText;

	public GameObject abilityIconPrefab;
	public GameObject errorMessagePrefab;

	TurnManager turnManager;
	UserInterfaceManager uIManager;

	List<GameObject> abilityIcons = new List<GameObject>();
	Dictionary<string, RuntimeAnimatorController> abilityIconControllers = new Dictionary<string, RuntimeAnimatorController>();

	Text objectivesBody;
	AbilityDescriptionController abilityDescription;
	UnitInfoController unitInfo;

	void Awake() {
		turnManager = GetComponentInParent<TurnManager> ();
		uIManager = GetComponentInParent<UserInterfaceManager> ();
		abilityDescription = transform.FindChild("AbilityDescription").GetComponent<AbilityDescriptionController> ();
		unitInfo = transform.FindChild("UnitInfo").GetComponent<UnitInfoController> ();
		unitInfo.HideWindow ();
	}

	// Use this for initialization
	void Start () {
		abilityDescription.HideWindow ();
	}

	// Update is called once per frame
	void Update () {
		//TODO this should probably be some sort of timer so there is less of a chance the user can get stuck
		if (showingTurn) {
			if (turnText == null) {
				turnManager.FinishStartingTurn ();
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
		turnText.transform.FindChild("AllyTurnImage").gameObject.SetActive(ally);
		turnText.transform.FindChild("EnemyTurnImage").gameObject.SetActive(!ally);

		if (objectivesBody == null) {
			objectivesBody = transform.FindChild ("ObjectivesBody").GetComponent<Text> ();
		}
		objectivesBody.text = CreateObjectiveText (objectives);

		//TODO THIS SHOULD FIND THE LENGTH OF THE ANIMATION AND NOT BE HARD CODED BUT IM TIRED
		Destroy (turnText, 1.917f);

	}

	public void UnitSelected(UnitController unit) {
		DisplayUnitAbilities (unit);
		ShowUnitInfo (unit);
	}

	public void UnitDeselected() {
		ClearAbilityIcons ();
		HideUnitInfo ();
		HideAbilityDescription ();
	}

	public void ClearAbilityIcons() {
		abilityIcons.ForEach ((icon) => {
			Destroy (icon.gameObject);
			AbilityDeselected(abilityIcons.IndexOf(icon));
		});
		abilityIcons.Clear ();
	}

	public void DisplayUnitAbilities(UnitController unit) {
		UnitClass unitClass = unit.GetComponent<UnitClass> ();

		unitClass.abilities.ForEach ((ability) => CreateAbilityIcon (ability));
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

	public void CreateAbilityIcon(BaseAbility ability) {
		GameObject newAbilityIcon = Instantiate (abilityIconPrefab);
		newAbilityIcon.GetComponent<Animator> ().runtimeAnimatorController = LoadRuntimeAnimatorController(ability.icon);
		Vector3 newPosition = newAbilityIcon.transform.position;
		newAbilityIcon.transform.SetParent(transform, false);
		newPosition.x = -35 + (abilityIcons.Count * 70);
		newAbilityIcon.GetComponent<RectTransform> ().anchoredPosition = newPosition;
		newAbilityIcon.GetComponent<AbilityIconController> ().Initialize (abilityIcons.Count, ability, uIManager, this);
		abilityIcons.Add(newAbilityIcon);
	}

	RuntimeAnimatorController LoadRuntimeAnimatorController(string controller) {
		if (!abilityIconControllers.ContainsKey(controller)) {
			abilityIconControllers.Add (controller, Resources.Load<RuntimeAnimatorController> ("Graphics/UI/InGame/Icons/" + controller));
		}

		return abilityIconControllers [controller];
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

	public void ShowAbilityDescription(BaseAbility ability) {
		abilityDescription.SetAbility (ability);
		abilityDescription.ShowWindow ();
	}

	public void HideAbilityDescription() {
		abilityDescription.HideWindow ();
	}

	public void ShowUnitInfo(UnitController unit) {
		unitInfo.SetUnit (unit);
		unitInfo.ShowWindow ();
	}

	public void HideUnitInfo() {
		unitInfo.HideWindow ();
	}
}
