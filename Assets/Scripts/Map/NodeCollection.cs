using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeCollection : Tile {
    private List<Node> nodes = new List<Node>();
    private Vector2 sumPosition = Vector2.zero;

    public void SetUnit(UnitController unit) {
        myUnit = unit;
        Nodes.ForEach((Node node) => node.MyUnit = unit);
    }

    public Tile First() {
        return Get(0);
    }

    public Node Get(int index) {
        return Nodes[index];
    }

    public void Add(Node node) {
        sumPosition += new Vector2(node.x, node.y);
        Nodes.Add(node);
        RecalculateValues();
    }

    public void Add(List<Node> newNodes) {
        newNodes.ForEach((Node node) => Add(node));
    }

    public void Set(Node node) {
        Clear();
        Add(node);
    }

    public void Set(List<Node> newNodes) {
        Clear();
        Add(newNodes);
    }

    public void Set(NodeCollection collection) {
        Clear();
        Add(collection.Nodes);
        SetUnit(collection.MyUnit);
    }

    public void Remove(Node node) {
        sumPosition -= new Vector2(node.x, node.y);
        Nodes.Remove(node);
        RecalculateValues();
    }

    public void Clear() {
        sumPosition = Vector2.zero;
        Nodes.Clear();
        RecalculateValues();
    }

    public bool Contains(Node node) {
        return Nodes.Contains(node);
    }

    public Neighbour FindNeighbourTo(Node target) {
        return Nodes.Find((Node node) => node.FindNeighbourTo(target) != null).FindNeighbourTo(target);
    }

    private Vector2 CalculatePosition() {
        return sumPosition / Nodes.Count;
    }

    private void RecalculateValues() {
        CalculateWalkable();
        CalculateMoveCost();
    }

    private void CalculateWalkable() {
        walkable = WalkableLevel.Walkable;
        Nodes.ForEach((Node node) => {
            if (node.Walkable > walkable) {
                walkable = node.Walkable;
            }
        });
    }

    private void CalculateMoveCost() {
        moveCost = 0;
        Nodes.ForEach((Node node) => {
            if (node.MoveCost > moveCost) {
                moveCost = node.MoveCost;
            }
        });
    }

    public override List<Node> Nodes {
        get { return nodes; }
    }

    public override UnitController MyUnit {
        get { return myUnit; }
        set { SetUnit(value); }
    }

    public override float X {
        get { return CalculatePosition().x; }
    }

    public override float Y {
        get { return CalculatePosition().y; }
    }

    public override Vector3 Position {
        get { return TileMap.getPositionOfNodes(this); }
    }

    public Transform Transform {
        get { return First().transform; }
    }

    public override WalkableLevel Walkable {
        get { return walkable; }
        set { }
    }

    public override float MoveCost {
        get { return moveCost; }
        set { }
    }
}