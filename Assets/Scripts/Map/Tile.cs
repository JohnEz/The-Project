﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

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

    [System.NonSerialized]
    protected UnitController myUnit;

    protected WalkableLevel walkable;
    protected float moveCost = 1;

    public virtual UnitController MyUnit {
        get { return myUnit; }
        set { myUnit = value; }
    }

    public virtual float X {
        get { return x; }
    }

    public virtual float Y {
        get { return y; }
    }

    public virtual bool Equals(Tile other) {
        return x == other.x && y == other.y;
    }

    public virtual Vector3 Position {
        get { return transform.position; }
    }

    public virtual List<Node> Nodes {
        get { return new List<Node> { (Node)this }; }
    }

    public virtual int GridDistanceTo(Tile t) {
        int diffX = t.x - x;
        int diffY = t.y - y;
        return Mathf.Abs(diffX) + Mathf.Abs(diffY);
    }

    public virtual Neighbour FindNeighbourTo(Tile target) {
        return neighbours.Find(neighbour => neighbour.GetOppositeTile(this) == target);
    }

    public virtual WalkableLevel Walkable {
        get { return walkable; }
        set { walkable = value; }
    }

    public virtual float MoveCost {
        get { return moveCost; }
        set { moveCost = value; }
    }
}