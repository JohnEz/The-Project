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

	public void SetEffectedTiles(List<Node> effectedTiles, SquareTarget targetType = SquareTarget.UNDEFINED, bool path = false) {
		ClearEffectedTiles ();
		int i = 0;

		effectedTiles.ForEach ((tile) => {
			TileHighlighter highlighter = tile.GetComponentInChildren<TileHighlighter> ();
			highlighter.EffectedTile = true;
			if (targetType != SquareTarget.UNDEFINED) {
				highlighter.highlight (targetType);
			}
			highlighter.showHighlight (true);
			if (path) {
				Vector2 nextDirection = i < effectedTiles.Count-1 ? effectedTiles[i+1].previous.direction : new Vector2(0, 0);
				//Vector2 previousDirection = i > 0 ? tile.previous.direction : new Vector2(0, 0);
				Vector2 previousDirection = tile.previous.direction;

				highlighter.ShowArrow(previousDirection, nextDirection); 
			}
			currentlyEffected.Add(tile);
			i++;
		});


	}

	public void ClearEffectedTiles() {
		currentlyEffected.ForEach ((tile) => {
			TileHighlighter highlighter = tile.GetComponentInChildren<TileHighlighter> ();
			highlighter.EffectedTile = false;
			if (!currentlyHighlighted.Contains(tile)) {
				highlighter.highlight (SquareTarget.NONE);
				highlighter.showHighlight (false);
			}
			highlighter.RemoveArrow();
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
