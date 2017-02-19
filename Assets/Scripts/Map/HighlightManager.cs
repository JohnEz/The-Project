using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HighlightManager : MonoBehaviour {

	List<Node> currentlyHighlighted = new List<Node>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void HighlightTiles(List<Node> tilesToHighlight, SquareTarget targetType) {
		foreach (Node n in tilesToHighlight) {
			currentlyHighlighted.Add (n);
			n.GetComponentInChildren<TileHighlighter> ().highlight (targetType);
			n.GetComponentInChildren<TileHighlighter> ().showHighlight (true);
		}
	}

	public void UnhighlightTiles() {
		foreach (Node n in currentlyHighlighted) {
			n.GetComponentInChildren<TileHighlighter> ().highlight (SquareTarget.NONE);
			n.GetComponentInChildren<TileHighlighter> ().showHighlight (false);
		}
		currentlyHighlighted = new List<Node>();
	}

	public void HighlightTile(Node tileToHighlight, SquareTarget targetType) {
		currentlyHighlighted.Add(tileToHighlight);
		tileToHighlight.GetComponentInChildren<TileHighlighter> ().highlight (targetType);
		tileToHighlight.GetComponentInChildren<TileHighlighter> ().showHighlight (true);
	}

	public void UnhighlightTile(Node tileToHighlight) {
		tileToHighlight.GetComponentInChildren<TileHighlighter> ().highlight (SquareTarget.NONE);
		tileToHighlight.GetComponentInChildren<TileHighlighter> ().showHighlight (false);
	}
}
