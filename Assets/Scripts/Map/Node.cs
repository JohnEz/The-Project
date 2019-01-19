﻿using System.Collections.Generic;
using UnityEngine;

public class NeighbourLegacy {
    public Vector2 direction;
    public Node node;
    public bool hasDoor;

    public override string ToString() {
        return string.Format("[ dirX: {0} dirY: {1} hasDoor: {2} ]", direction.x, direction.y, hasDoor);
    }
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
    public int height = 0;
    public int room = 0;

    //Game engine variables
    public UnitController myUnit;

    public Node previousMoveNode; //used for move and attack

    // Use this for initialization
    private void Start() {
    }

    // Update is called once per frame
    private void Update() {
    }

    public float distanceTo(Node n) {
        return Vector2.Distance(new Vector2(x, y), new Vector2(n.x, n.y));
    }

    public float Value {
        get { return cost + dist; }
    }

    public void Reset() {
        cost = Mathf.Infinity;
        previous = null;
        previousMoveNode = null;
    }

    public bool HasDoor() {
        return neighbours.Exists(n => n.HasDoor());
    }

    public void OpenDoors() {
        neighbours.ForEach(n => {
            if (n.HasDoor()) {
                n.OpenDoor();
            }
        });
    }

    public void Activate() {
        if (myUnit != null) {
            myUnit.myStats.isActive = true;
        }
    }

    public Neighbour FindNeighbourTo(Node target) {
        return neighbours.Find(neighbour => neighbour.GetOppositeNode(this) == target);
    }

    public override string ToString() {
        return string.Format("[ X: {0} Y: {1} MyUnit: {2} ]", x, y, myUnit != null ? myUnit.name : "None");
    }
}