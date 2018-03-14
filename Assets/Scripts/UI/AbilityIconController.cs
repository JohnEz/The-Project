using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityIconController : MonoBehaviour, IPointerClickHandler {

	UserInterfaceManager uIManager;
	int index;
	BaseAbility myAbility;

	bool onCooldown = false;
	Color fadedColor = new Color (0.33f, 0.33f, 0.33f);

	public void Initialize(int _index, BaseAbility _myAbility, UserInterfaceManager _uIManager) {
		uIManager = _uIManager;
		index = _index;
		myAbility = _myAbility;
	}

	public void OnPointerClick(PointerEventData e) {
		uIManager.ShowAbility (index);
	}

	void Update () {
		if (onCooldown != myAbility.IsOnCooldown ()) {
			onCooldown = myAbility.IsOnCooldown ();
			GetComponent<Image> ().color = onCooldown ? fadedColor : Color.white;
			GetComponentInChildren<Text> ().text = myAbility.cooldown.ToString();
		}
	}

	void OnMouseEnter() {
		
	}

	void OnMouseExit() {
		
	}
}
