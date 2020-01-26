using System.Collections.Generic;
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

    public int GridDistanceTo(NodeCollection collection) {
        //TODO double check the outcome of this int rounding
        int diffX = (int)collection.x - x;
        int diffY = (int)collection.y - y;
        return Mathf.Abs(diffX) + Mathf.Abs(diffY);
    }

    public bool HasDoor() {
        return neighbours.Exists(n => n.HasDoor());
    }

    public void Activate() {
        if (myUnit != null) {
            myUnit.Activate();
        }
    }

    public override string ToString() {
        return string.Format("[ X: {0} Y: {1} MyUnit: {2} ]", x, y, myUnit != null ? myUnit.name : "None");
    }
}