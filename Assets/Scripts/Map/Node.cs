using UnityEngine;
using System.Collections.Generic;

public struct Neighbour {
	public Vector2 direction;
	public Node node;
}

public enum Walkable {
	Walkable,
	Flying,
	Impassable
}

public enum LineOfSight {
    Full,
    Blocked
}

public class Node : MonoBehaviour {

	[System.NonSerialized]
	public int x;
	[System.NonSerialized]
	public int y;

	[System.NonSerialized]
	public List<Neighbour> neighbours;
	[System.NonSerialized]
	public Neighbour previous;
	[System.NonSerialized]
	public float cost = 0;
	[System.NonSerialized]
	public float dist = 0;
	public float moveCost = 1;
	public Walkable walkable;
    public LineOfSight lineOfSight = LineOfSight.Full;
	public int level = 0;

	//Game engine variables
	public UnitController myUnit;
	public Node previousMoveNode; //used for move and attack


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public float distanceTo(Node n) {
		return Vector2.Distance (new Vector2(x, y), new Vector2(n.x, n.y));
	}

	public float Value {
		get { return cost + dist; }
	}

	public void Reset() {
		cost = Mathf.Infinity;
		previous = new Neighbour();
		previousMoveNode = null;
	}

    public override string ToString() {
        return string.Format("[ X: {0} Y: {1} MyUnit: {2} ]", x, y, myUnit != null ? myUnit.name : "None");
    }
}

