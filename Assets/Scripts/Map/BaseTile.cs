using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class BaseTile : MonoBehaviour, 
IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

	public void OnPointerClick(PointerEventData eventData) {
		if (eventData.button == PointerEventData.InputButton.Left) {
			GetComponentInChildren<TileHighlighter> ().OnMouseUp ();
		}
	}

	public void OnPointerEnter(PointerEventData eventData) {
		GetComponentInChildren<TileHighlighter> ().OnMouseEnter();
	}

	public void OnPointerExit(PointerEventData eventData) {
		GetComponentInChildren<TileHighlighter> ().OnMouseExit();
	}
}
