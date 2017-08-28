using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public enum SquareTarget {
	NONE,
	MOVEMENT,
	DASH,
	HELPFULL,
	ATTACK
}

public class TileHighlighter : MonoBehaviour {

	public Sprite highlightedSprite;
	public Sprite hoverSprite;

	Color blue = new Color (0, 0.9647f, 1);
	Color red = new Color (0.8431f, 0.2f, 0.2f);
	Color green = new Color (0.7294f, 0.9569f, 0.1176f);
	Color yellow = new Color (0.9569f, 0.7294f, 0.1176f);
	Color white = new Color (1, 1, 1);

	bool showingHighlight = false;
	bool hovered = false;

	float maxAlpha = 0.5f;
	float minAlpha = 0.05f;

	SquareTarget myTarget = SquareTarget.NONE;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
		
	public void OnMouseUp ()
	{
		Node myNode = GetComponentInParent<Node> ();
		if (hovered) {
			GetComponentInParent<TurnManager> ().TileClicked (myNode, myTarget);
		}
	}

	public void OnMouseEnter() {
		SpriteRenderer mySprite = GetComponent<SpriteRenderer> ();
		mySprite.sprite = hoverSprite;
		if (!showingHighlight) {
			updateAlpha (maxAlpha);
		}
		hovered = true;
	}

	public void OnMouseExit() {
		SpriteRenderer mySprite = GetComponent<SpriteRenderer> ();
		mySprite.sprite = highlightedSprite;
		if (!showingHighlight) {
			updateAlpha (minAlpha);
		}
		hovered = false;
	}

	public void showHighlight(bool show) {
		showingHighlight = show;
		float newAlpha = maxAlpha;

		if (!show) {
			newAlpha = minAlpha;
		}

		updateAlpha (newAlpha);
	}

	public void updateAlpha(float alpha) {
		SpriteRenderer mySprite = GetComponent<SpriteRenderer> ();
		Color newColour = mySprite.color;

		newColour.a = alpha;

		mySprite.color = newColour;
	}

	public void highlight(SquareTarget targetType) {
		SpriteRenderer mySprite = GetComponent<SpriteRenderer> ();

		float startAlpha = mySprite.color.a;

		myTarget = targetType;

		switch (targetType) {
		case SquareTarget.MOVEMENT:
			mySprite.color = blue;
			break;
		case SquareTarget.DASH:
			mySprite.color = yellow;
			break;
		case SquareTarget.HELPFULL:
			mySprite.color = green;
			break;
		case SquareTarget.ATTACK:
			mySprite.color = red;
			break;
		case SquareTarget.NONE:
		default: 
			mySprite.color = white;
			break;
		}

		updateAlpha (startAlpha);
	}

}
