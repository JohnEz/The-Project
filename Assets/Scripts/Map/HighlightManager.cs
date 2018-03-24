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

	public void ShowPath(List<Node> effectedTiles) {
		SetEffectedTiles (effectedTiles, SquareTarget.UNDEFINED, true);
	}

	public void ShowAbilityTiles(List<Node> effectedTiles, BaseAbility ability) {
		SquareTarget targetType = ability.targets == TargetType.ALLY ? SquareTarget.HELPFULL : SquareTarget.ATTACK;
		SetEffectedTiles (effectedTiles, targetType, false, ability);
	}

	public void SetEffectedTiles(List<Node> effectedTiles, SquareTarget targetType = SquareTarget.UNDEFINED, bool path = false, BaseAbility ability = null) {
		ClearEffectedTiles ();
		int i = 0;

		effectedTiles.ForEach ((tile) => {
			TileHighlighter highlighter = tile.GetComponentInChildren<TileHighlighter> ();
			highlighter.SetEffected(true);
			if (targetType != SquareTarget.UNDEFINED) {
				highlighter.SetTargetType (targetType);
			}
			highlighter.SetHighlighted (true);

			if (tile.myUnit != null && ability != null && ability.CanHitUnit(tile)) {
				highlighter.AddDecal(SquareDecal.TARGET);
			}
			if (path) {
				Vector2 nextDirection = i < effectedTiles.Count-1 ? effectedTiles[i+1].previous.direction : new Vector2(0, 0);
				//Vector2 previousDirection = i > 0 ? tile.previous.direction : new Vector2(0, 0);
				Vector2 previousDirection = tile.previous.direction;

				highlighter.CreateArrowDecal(previousDirection, nextDirection); 
			}
			currentlyEffected.Add(tile);
			i++;
		});


	}

	public void ClearEffectedTiles() {
		currentlyEffected.ForEach ((tile) => {
			TileHighlighter highlighter = tile.GetComponentInChildren<TileHighlighter> ();
			highlighter.SetEffected(false);
			if (!currentlyHighlighted.Contains(tile)) {
				highlighter.CleanHighlight ();
			}
			highlighter.ClearDecals();
		});
		currentlyEffected.Clear ();
	}

	public void HighlightTiles(List<Node> tilesToHighlight, SquareTarget targetType) {
		foreach (Node n in tilesToHighlight) {
			currentlyHighlighted.Add (n);
			TileHighlighter highlighter = n.GetComponentInChildren<TileHighlighter> ();
			highlighter.SetTargetType (targetType);
			highlighter.SetHighlighted (true);
		}
	}

	public void HighlightTile(Node tileToHighlight, SquareTarget targetType) {
		currentlyHighlighted.Add(tileToHighlight);
		tileToHighlight.GetComponentInChildren<TileHighlighter> ().SetTargetType (targetType);
		tileToHighlight.GetComponentInChildren<TileHighlighter> ().SetHighlighted (true);
	}

	public void UnhighlightTiles() {
		foreach (Node n in currentlyHighlighted) {
			UnhighlightTile(n);
		}
		currentlyHighlighted = new List<Node>();
	}

	public void UnhighlightAllTiles() {
		UnhighlightTiles ();
		ClearEffectedTiles ();
	}

	public void UnhighlightTile(Node tileToHighlight) {
		tileToHighlight.GetComponentInChildren<TileHighlighter> ().CleanHighlight ();
	}
}
