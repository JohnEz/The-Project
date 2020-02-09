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

public enum WalkableLevel {
    Walkable,
    Flying,
    Impassable
}

public enum LineOfSight {
    Full,
    Blocked
}

public class Node : Tile {
    public LineOfSight lineOfSight = LineOfSight.Full;
    public int height = 0;
    public int room = 0;

    public Node previousMoveNode; //used for move and attack

    public float distanceTo(Node n) {
        return Vector2.Distance(new Vector2(x, y), new Vector2(n.x, n.y));
    }

    public bool HasDoor() {
        return neighbours.Exists(n => n.HasDoor());
    }

    public void Activate() {
        if (myUnit != null) {
            myUnit.Activate();
        }
    }
}