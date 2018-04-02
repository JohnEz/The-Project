using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Reflection;
using System;

public enum SquareTarget {
	UNDEFINED,
	NONE,
	MOVEMENT,
	DASH,
	HELPFULL,
	ATTACK
}

public enum SquareDecal {
	NONE,
	TARGET,
	ARROW
}

struct TileState {
	public bool hovered;
	public bool highlighted;
	public bool effected;

	public void SetValue(string key, bool value) {
		//TODO there must be a clean way to do this in c#
		switch (key) {
		case "hovered":
			hovered = value;
			break;
		case "highlighted":
			highlighted = value;
			break;
		case "effected":
			effected = value;
			break;
		}
	}
}

public class TileHighlighter : MonoBehaviour {

	//Highlights
	public Sprite basicSprite;
	public Sprite highlightedSprite;
	public Sprite effectedSprite;
	public Sprite hoverSprite;

	//Decals
	public Sprite targetDecal;
	public Sprite arrowStraight;
	public Sprite arrowCorner;
	public Sprite arrowEnd;

	public GameObject tileDecalPrefab;

	Color blue = new Color (0, 0.9647f, 1);
	Color red = new Color (0.8431f, 0.2f, 0.2f);
	Color green = new Color (0.7294f, 0.9569f, 0.1176f);
	Color yellow = new Color (0.9569f, 0.7294f, 0.1176f);
	Color white = new Color (1, 1, 1);

	TileState myState;

	public SquareTarget myTarget = SquareTarget.NONE;

	SpriteRenderer mySpriteRenderer;

	List<GameObject> myDecals = new List<GameObject>();

	// Use this for initialization
	void Start () {
		
	}

	public void Initialise() {
		myState = new TileState ();
		myState.effected = false;
		myState.highlighted = false;
		myState.hovered = false;
		mySpriteRenderer = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		
	public void OnMouseUp ()
	{
		Node myNode = GetComponentInParent<Node> ();
		if (myState.hovered) {
			GetComponentInParent<UserInterfaceManager> ().TileClicked (myNode, myTarget);
		}
	}

	public void OnMouseEnter() {
		UpdateState("hovered", true);
		Node myNode = GetComponentInParent<Node> ();
		GetComponentInParent<UserInterfaceManager> ().TileHovered (myNode, myTarget);
	}

	public void OnMouseExit() {
		UpdateState("hovered", false);
		Node myNode = GetComponentInParent<Node> ();
		GetComponentInParent<UserInterfaceManager> ().TileExit (myNode, myTarget);
	}

	public void SetHighlighted(bool highlighted) {
		UpdateState("highlighted", highlighted);
	}

	public void SetEffected(bool effected) {
		UpdateState("effected", effected);
	}

	public void CleanHighlight() {
		myState.effected = false;
		myState.highlighted = false;
		mySpriteRenderer.sprite = GetCurrentHighlight();
		mySpriteRenderer.color = white;
		myTarget = SquareTarget.NONE;
		ClearDecals ();
	}

	public void UpdateState(string key, bool value) {
		myState.SetValue (key, value);
		mySpriteRenderer.sprite = GetCurrentHighlight();
	}

	public Sprite GetCurrentHighlight() {
		if (myState.hovered) {
			return hoverSprite;
		} else if (myState.effected) {
			return effectedSprite;
		} else if (myState.highlighted) {
			return highlightedSprite;
		} else {
			return basicSprite;
		}
	}

	public void updateAlpha(float alpha) {
		Color newColour = mySpriteRenderer.color;

		newColour.a = alpha;

		mySpriteRenderer.color = newColour;
	}

	public void SetTargetType(SquareTarget targetType) {
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
	}

	public void CreateArrowDecal(Vector2 previous, Vector2 next) {
		Sprite arrowSprite;
		Vector3 rotation = new Vector3();
		Vector2 minusDirections = next - previous;

		//end arrow
		if (next == new Vector2 (0, 0)) {
			arrowSprite = arrowEnd;

			if (previous.x == -1) {
				rotation = new Vector3 (0, 0, -180);
			} else if (previous.y == -1) {
				rotation = new Vector3 (0, 0, -90);
			} else if (previous.y == 1) {
				rotation = new Vector3 (0, 0, 90);
			}
			//sort rotation
		} else if (minusDirections == new Vector2 (0, 0)) {
			arrowSprite = arrowStraight;
			if (previous.y != 0) {
				rotation = new Vector3 (0, 0, -90);
			}
		} else {
			arrowSprite = arrowCorner;

			if (minusDirections.x == -1 && minusDirections.y == -1) {
				rotation = new Vector3 (0, 0, -90);
			} else if (minusDirections.x == 1 && minusDirections.y == 1) {
				rotation = new Vector3 (0, 0, 90);
			} else if (minusDirections.x == -1 && minusDirections.y == 1) {
				rotation = new Vector3 (0, 0, 180);
			}
		}
			
		CreateDecal (arrowSprite, rotation);
	}

	public void ClearDecals() {
		myDecals.ForEach (decal => {
			Destroy(decal);
		});
		myDecals.Clear ();
	}

	public void AddDecal(SquareDecal decal) {
		switch(decal) {
		case SquareDecal.TARGET:
			CreateDecal(targetDecal);
			break;
		}
	}

	void CreateDecal(Sprite decalSprite, Vector3 rotation = new Vector3()) {
		GameObject newDecal = Instantiate (tileDecalPrefab);
		newDecal.GetComponent<SpriteRenderer> ().sprite = decalSprite;
		newDecal.transform.SetParent(this.transform, false);
		newDecal.transform.Rotate (rotation);
		myDecals.Add (newDecal);
	}

}
