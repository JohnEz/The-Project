using System.Collections.Generic;
using UnityEngine;

public class HighlightManager : MonoBehaviour {
    public static HighlightManager instance;

    private void Awake() {
        instance = this;
    }

    public List<Node> GetHighlightedNodes() {
        return TileMap.instance.GetNodes().FindAll(node => node.GetComponentInChildren<TileHighlighter>().GetHighlighted());
    }

    public List<Node> GetEffectedNodes() {
        return TileMap.instance.GetNodes().FindAll(node => node.GetComponentInChildren<TileHighlighter>().GetEffected());
    }

    public void ShowPath(List<Tile> effectedTiles) {
        List<Node> effectedNodes = new List<Node>();

        effectedTiles.ForEach((Tile tile) => {
            tile.Nodes.ForEach((Node node) => {
                if (!effectedNodes.Contains(node)) {
                    effectedNodes.Add(node);
                }
            });
        });

        SetEffectedNodes(effectedNodes, SquareTarget.UNDEFINED, true);
    }

    public void ShowAbilityNodes(List<Node> effectedNodes, AttackAction action) {
        SquareTarget targetType = action.targets == TargetType.ALLY ? SquareTarget.HELPFULL : SquareTarget.ATTACK;
        SetEffectedNodes(effectedNodes, targetType, false, action);
    }

    // TODO, re write this trash
    public void SetEffectedNodes(List<Node> effectedNodes, SquareTarget targetType = SquareTarget.UNDEFINED, bool path = false, AttackAction action = null) {
        ClearEffectedNodes();
        int i = 0;

        effectedNodes.ForEach((node) => {
            TileHighlighter highlighter = node.GetComponentInChildren<TileHighlighter>();

            SetEffectedNode(node, targetType);

            if (path) {
                // if we are not at the end node, set the next direction to the direction between this and the next node
                Vector2 previousDirection = node.previous.GetDirectionFrom(node);
                Vector2 nextDirection = i == effectedNodes.Count - 1 ? new Vector2(0, 0) : effectedNodes[i + 1].previous.GetDirectionFrom(effectedNodes[i + 1]);

                highlighter.CreateArrowDecal(previousDirection, nextDirection);
            }
            i++;
        });
    }

    public void SetEffectedNode(Node nodeToHighlight, SquareTarget targetType) {
        TileHighlighter highlighter = nodeToHighlight.GetComponentInChildren<TileHighlighter>();
        highlighter.SetEffected(true);

        if (targetType != SquareTarget.UNDEFINED) {
            highlighter.SetTargetType(targetType);
        }
    }

    public void ClearEffectedNodes(bool clearSelectedUnit = false) {
        List<Node> currentlyHighlightedNodes = GetHighlightedNodes();
        GetEffectedNodes().ForEach((node) => {
            TileHighlighter highlighter = node.GetComponentInChildren<TileHighlighter>();
            if (!clearSelectedUnit && highlighter.myTarget == SquareTarget.SELECTED_UNIT) {
                return;
            }

            highlighter.SetEffected(false);
            if (!currentlyHighlightedNodes.Contains(node)) {
                highlighter.CleanHighlight();
            }
            highlighter.ClearDecals();
        });
    }

    public void AddDecal(List<Node> nodesToHighlight, SquareDecal decal) {
        foreach (Node n in nodesToHighlight) {
            TileHighlighter highlighter = n.GetComponentInChildren<TileHighlighter>();
            highlighter.AddDecal(decal);
        }
    }

    public void RemoveDecals(List<Node> nodesToHighlight) {
        foreach (Node n in nodesToHighlight) {
            TileHighlighter highlighter = n.GetComponentInChildren<TileHighlighter>();
            highlighter.ClearDecals();
        }
    }

    public void HighlightNodes(List<Node> nodesToHighlight, SquareTarget targetType) {
        foreach (Node n in nodesToHighlight) {
            TileHighlighter highlighter = n.GetComponentInChildren<TileHighlighter>();
            highlighter.SetTargetType(targetType);
            highlighter.SetHighlighted(true);
        }
    }

    public void HighlightNode(Node nodeToHighlight, SquareTarget targetType) {
        nodeToHighlight.GetComponentInChildren<TileHighlighter>().SetTargetType(targetType);
        nodeToHighlight.GetComponentInChildren<TileHighlighter>().SetHighlighted(true);
    }

    public void UnhighlightNodes() {
        foreach (Node n in GetHighlightedNodes()) {
            UnhighlightNode(n);
        }
    }

    public void UnhighlightAllNodes() {
        UnhighlightNodes();
        ClearEffectedNodes();
    }

    public void UnhighlightNodes(List<Node> nodesToUnhighlight) {
        foreach (Node n in nodesToUnhighlight) {
            UnhighlightNode(n);
        }
    }

    public void UnhighlightNode(Node nodeToHighlight) {
        nodeToHighlight.GetComponentInChildren<TileHighlighter>().CleanHighlight();
    }
}