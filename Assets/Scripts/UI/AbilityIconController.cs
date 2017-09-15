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

	public void OnPointerClick(PointerEventData e) {
		uIManager.ShowAbility (index);
	}

	void OnMouseEnter() {
		
	}

	void OnMouseExit() {
		
	}
}
