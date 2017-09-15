using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class AbilityIconController : MonoBehaviour, IPointerClickHandler {

	UserInterfaceManager uIManager;
	int index;

	public void Initialize(int _index, UserInterfaceManager _uIManager) {
		uIManager = _uIManager;
		index = _index;
	}

	void OnMouseUp() {
		Debug.Log ("Clicked: " + index.ToString ());
		uIManager.ShowAbility (index);
	}

	public void OnPointerClick(PointerEventData e) {
		Debug.Log ("Clicked: " + index.ToString ());
		uIManager.ShowAbility (index);
	}

	void OnMouseEnter() {
		
	}

	void OnMouseExit() {
		
	}
}
