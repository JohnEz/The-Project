using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HighlightManager : MonoBehaviour {

	List<Node> currentlyHighlighted = new List<Node>();
	List<Node> currentlyEffected = new List<Node>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetEffectedTiles(List<Node> effectedTiles, SquareTarget targetType = SquareTarget.UNDEFINED) {
		ClearEffectedTiles ();

		effectedTiles.ForEach ((tile) => {
			tile.GetComponentInChildren<TileHighlighter> ().EffectedTile = true;
			if (targetType != SquareTarget.UNDEFINED) {
				tile.GetComponentInChildren<TileHighlighter> ().highlight (targetType);
			}
			tile.GetComponentInChildren<TileHighlighter> ().showHighlight (true);
			currentlyEffected.Add(tile);
		});
	}

	public void ClearEffectedTiles() {
		currentlyEffected.ForEach ((tile) => {
			tile.GetComponentInChildren<TileHighlighter> ().EffectedTile = false;
			if (!currentlyHighlighted.Contains(tile)) {
				tile.GetComponentInChildren<TileHighlighter> ().highlight (SquareTarget.NONE);
				tile.GetComponentInChildren<TileHighlighter> ().showHighlight (false);
			}
		});
		currentlyEffected.Clear ();
	}

	public void HighlightTiles(List<Node> tilesToHighlight, SquareTarget targetType) {
		foreach (Node n in tilesToHighlight) {
			currentlyHighlighted.Add (n);
			n.GetComponentInChildren<TileHighlighter> ().highlight (targetType);
			n.GetComponentInChildren<TileHighlighter> ().showHighlight (true);
		}
	}

	public void HighlightTile(Node tileToHighlight, SquareTarget targetType) {
		currentlyHighlighted.Add(tileToHighlight);
		tileToHighlight.GetComponentInChildren<TileHighlighter> ().highlight (targetType);
		tileToHighlight.GetComponentInChildren<TileHighlighter> ().showHighlight (true);
	}

	public void UnhighlightTiles() {
		foreach (Node n in currentlyHighlighted) {
			n.GetComponentInChildren<TileHighlighter> ().highlight (SquareTarget.NONE);
			n.GetComponentInChildren<TileHighlighter> ().showHighlight (false);
		}
		currentlyHighlighted = new List<Node>();
	}

	public void UnhighlightAllTiles() {
		UnhighlightTiles ();
		ClearEffectedTiles ();
	}

	public void UnhighlightTile(Node tileToHighlight) {
		tileToHighlight.GetComponentInChildren<TileHighlighter> ().highlight (SquareTarget.NONE);
		tileToHighlight.GetComponentInChildren<TileHighlighter> ().showHighlight (false);
	}
}
