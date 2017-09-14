using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public enum SquareTarget {
	UNDEFINED,
	NONE,
	MOVEMENT,
	DASH,
	HELPFULL,
	ATTACK
}

public class TileHighlighter : MonoBehaviour {

	public Sprite highlightSprite;
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

	public SquareTarget myTarget = SquareTarget.NONE;

	bool effectedTile = false;

	SpriteRenderer mySpriteRenderer;

	// Use this for initialization
	void Start () {
		mySpriteRenderer = GetComponent<SpriteRenderer> ();
	}

	public void Initialise() {
		mySpriteRenderer = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool EffectedTile {
		get { return effectedTile; }
		set { 
			if (!hovered) {
				mySpriteRenderer.sprite = value ? highlightedSprite : highlightSprite;
			}
			effectedTile = value; 
		}
	}
		
	public void OnMouseUp ()
	{
		Node myNode = GetComponentInParent<Node> ();
		if (hovered) {
			GetComponentInParent<UserInterfaceManager> ().TileClicked (myNode, myTarget);
		}
	}

	public void OnMouseEnter() {
		mySpriteRenderer.sprite = hoverSprite;
		if (!showingHighlight) {
			updateAlpha (maxAlpha);
		}
		hovered = true;
		Node myNode = GetComponentInParent<Node> ();
		GetComponentInParent<UserInterfaceManager> ().TileHovered (myNode, myTarget);
	}

	public void OnMouseExit() {
		mySpriteRenderer.sprite = EffectedTile ? highlightedSprite : highlightSprite;
		if (!showingHighlight) {
			updateAlpha (minAlpha);
		}
		hovered = false;
		Node myNode = GetComponentInParent<Node> ();
		GetComponentInParent<UserInterfaceManager> ().TileExit (myNode, myTarget);
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
		Color newColour = mySpriteRenderer.color;

		newColour.a = alpha;

		mySpriteRenderer.color = newColour;
	}

	public void highlight(SquareTarget targetType) {
		float startAlpha = mySpriteRenderer.color.a;

		myTarget = targetType;

		switch (targetType) {
		case SquareTarget.MOVEMENT:
			mySpriteRenderer.color = blue;
			break;
		case SquareTarget.DASH:
			mySpriteRenderer.color = yellow;
			break;
		case SquareTarget.HELPFULL:
			mySpriteRenderer.color = green;
			break;
		case SquareTarget.ATTACK:
			mySpriteRenderer.color = red;
			break;
		case SquareTarget.NONE:
		default: 
			mySpriteRenderer.color = white;
			break;
		}

		updateAlpha (startAlpha);
	}

}
