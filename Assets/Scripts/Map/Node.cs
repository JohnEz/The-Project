using UnityEngine;
using System.Collections.Generic;

public struct Neighbour {
	public Vector2 direction;
	public Node node;
}

public enum Walkable {
	Impassable,
	Flying,
	Walkable
}

public class Node : MonoBehaviour {

	[System.NonSerialized]
	public int x;
	[System.NonSerialized]
	public int y;

	[System.NonSerialized]
	public Neighbour[] neighbours;
	[System.NonSerialized]
	public Neighbour previous;
	[System.NonSerialized]
	public float cost = Mathf.Infinity;
	[System.NonSerialized]
	public float dist = 0;
	public float moveCost = 0;
	public Walkable walkable;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public float distanceTo(Node n) {
		return Vector2.Distance (new Vector2(x, y), new Vector2(n.x, n.y));
	}
}

