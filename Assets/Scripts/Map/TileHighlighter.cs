using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum SquareTarget {
    UNDEFINED,
    NONE,
    MOVEMENT,
    DASH,
    HELPFULL,
    SELECTED_UNIT,
    ATTACK
}

public enum SquareDecal {
    NONE,
    TARGET,
    ARROW,
    NO_SIGHT,
}

//TODO SORT THIS TRASH WHY TF DIDNT I USE SOME FORM OF DICTIONARY?
internal struct TileState {
    public const string HOVERED = "hovered";
    public const string HIGHLIGHTED = "highlighted";
    public const string EFFECTED = "effected";

    public bool hovered;
    public bool highlighted;
    public bool effected;

    public void SetValue(string key, bool value) {
        //TODO there must be a clean way to do this in c#
        switch (key) {
            case HOVERED:
                hovered = value;
                break;

            case HIGHLIGHTED:
                highlighted = value;
                break;

            case EFFECTED:
                effected = value;
                break;
        }
    }

    public bool GetValue(string key) {
        //TODO there must be a clean way to do this in c#
        switch (key) {
            case HOVERED:
                return hovered;

            case HIGHLIGHTED:
                return highlighted;

            case EFFECTED:
                return effected;
        }
        return false;
    }
}

public class TileHighlighter : MonoBehaviour {
    //Colours

    private Color blue = new Color(0, 0.9647f, 1);
    private Color red = new Color(0.8431f, 0.2f, 0.2f);
    private Color green = new Color(0.7294f, 0.9569f, 0.1176f);
    private Color yellow = new Color(0.9569f, 0.7294f, 0.1176f);
    private Color white = new Color(1, 1, 1);

    //Highlights

    public Sprite basicSprite;
    public Sprite highlightedSprite;
    public Sprite effectedSprite;
    public Sprite hoverSprite;

    //Decals

    public Sprite noVisionDecal;
    public Sprite targetDecal;
    public Sprite arrowStraight;
    public Sprite arrowCorner;
    public Sprite arrowEnd;

    public GameObject tileDecalPrefab;

    private TileState myState;

    public SquareTarget myTarget = SquareTarget.NONE;

    private SpriteRenderer mySpriteRenderer;

    private List<GameObject> myDecals = new List<GameObject>();

    // Use this for initialization
    private void Start() {
    }

    public void Initialise() {
        myState = new TileState();
        myState.effected = false;
        myState.highlighted = false;
        myState.hovered = false;
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update() {
    }

    public void OnMouseUp() {
        Node myNode = GetComponentInParent<Node>();
        if (myState.hovered) {
            UserInterfaceManager.instance.TileClicked(myNode, myTarget);
        }
    }

    public void OnMouseEnter() {
        UpdateState(TileState.HOVERED, true);
        Node myNode = GetComponentInParent<Node>();
        UserInterfaceManager.instance.TileHovered(myNode, myTarget);
    }

    public void OnMouseExit() {
        UpdateState(TileState.HOVERED, false);
        Node myNode = GetComponentInParent<Node>();
        UserInterfaceManager.instance.TileExit(myNode, myTarget);
    }

    public bool GetHighlighted() {
        return myState.GetValue(TileState.HIGHLIGHTED);
    }

    public void SetHighlighted(bool highlighted) {
        UpdateState(TileState.HIGHLIGHTED, highlighted);
    }

    public bool GetEffected() {
        return myState.GetValue(TileState.EFFECTED);
    }

    public void SetEffected(bool effected) {
        UpdateState(TileState.EFFECTED, effected);
    }

    public void CleanHighlight() {
        SetEffected(false);
        SetHighlighted(false);
        mySpriteRenderer.color = white;
        myTarget = SquareTarget.NONE;
        ClearDecals();
    }

    public void UpdateState(string key, bool value) {
        myState.SetValue(key, value);
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
        // work out what direction its going
        Vector2 minusDirections = next - previous;

        //end arrow
        if (next == new Vector2(0, 0)) {
            arrowSprite = arrowEnd;

            if (previous.x == -1) {
                rotation = new Vector3(0, 0, -180);
            } else if (previous.y == -1) {
                rotation = new Vector3(0, 0, -90);
            } else if (previous.y == 1) {
                rotation = new Vector3(0, 0, 90);
            }
        } else if (minusDirections == new Vector2(0, 0)) {
            // if the directions are 0,0 it means its not changing direction
            arrowSprite = arrowStraight;
            if (previous.y != 0) {
                rotation = new Vector3(0, 0, -90);
            }
        } else {
            arrowSprite = arrowCorner;

            if (minusDirections.x == -1 && minusDirections.y == -1) {
                rotation = new Vector3(0, 0, -90);
            } else if (minusDirections.x == 1 && minusDirections.y == 1) {
                rotation = new Vector3(0, 0, 90);
            } else if (minusDirections.x == -1 && minusDirections.y == 1) {
                rotation = new Vector3(0, 0, 180);
            }
        }

        CreateDecal(arrowSprite, rotation);
    }

    public void ClearDecals() {
        myDecals.ForEach(decal => {
            Destroy(decal);
        });
        myDecals.Clear();
    }

    public void ClearPathDecals() {
    }

    public void AddDecal(SquareDecal decal) {
        switch (decal) {
            case SquareDecal.TARGET:
                CreateDecal(targetDecal);
                break;

            case SquareDecal.NO_SIGHT:
                CreateDecal(noVisionDecal);
                break;
        }
    }

    private void CreateDecal(Sprite decalSprite, Vector3 rotation = new Vector3()) {
        GameObject newDecal = Instantiate(tileDecalPrefab);
        newDecal.GetComponent<SpriteRenderer>().sprite = decalSprite;
        newDecal.transform.SetParent(this.transform, false);
        newDecal.transform.Rotate(rotation);
        myDecals.Add(newDecal);
    }

    public void DebugSetColour(float alpha, Color color) {
        updateAlpha(alpha);
        mySpriteRenderer.color = color;
    }

    public void DebugSetText(string text) {
        transform.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }
}