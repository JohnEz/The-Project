using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class BaseTile : MonoBehaviour {

	void OnMouseUp() {
		GetComponentInChildren<TileHighlighter> ().OnMouseUp();
	}

	void OnMouseEnter() {
		GetComponentInChildren<TileHighlighter> ().OnMouseEnter();
	}

	void OnMouseExit() {
		GetComponentInChildren<TileHighlighter> ().OnMouseExit();
	}
}
