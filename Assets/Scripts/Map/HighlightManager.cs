﻿using System.Collections.Generic;
using UnityEngine;

public class HighlightManager : MonoBehaviour {
    public static HighlightManager instance;

    private List<Node> currentlyHighlighted = new List<Node>();
    private List<Node> currentlyEffected = new List<Node>();

    private void Awake() {
        instance = this;
    }

    // Use this for initialization
    private void Start() {
    }

    // Update is called once per frame
    private void Update() {
    }

    public void ShowPath(List<Node> effectedTiles) {
        SetEffectedTiles(effectedTiles, SquareTarget.UNDEFINED, true);
    }

    public void ShowAbilityTiles(List<Node> effectedTiles, AttackAction action) {
        SquareTarget targetType = action.targets == TargetType.ALLY ? SquareTarget.HELPFULL : SquareTarget.ATTACK;
        SetEffectedTiles(effectedTiles, targetType, false, action);
    }

    // TODO, re write this trash
    public void SetEffectedTiles(List<Node> effectedTiles, SquareTarget targetType = SquareTarget.UNDEFINED, bool path = false, AttackAction action = null) {
        ClearEffectedTiles();
        int i = 0;

        effectedTiles.ForEach((tile) => {
            TileHighlighter highlighter = tile.GetComponentInChildren<TileHighlighter>();

            SetEffectedTile(tile, targetType);

            if (path) {
                // if we are not at the end node, set the next direction to the direction between this and the next node
                Vector2 previousDirection = tile.previous.GetDirectionFrom(tile);
                Vector2 nextDirection = i == effectedTiles.Count - 1 ? new Vector2(0, 0) : effectedTiles[i + 1].previous.GetDirectionFrom(effectedTiles[i + 1]);

                highlighter.CreateArrowDecal(previousDirection, nextDirection);
            }
            i++;
        });
    }

    public void SetEffectedTile(Node tileToHighlight, SquareTarget targetType) {
        currentlyEffected.Add(tileToHighlight);
        TileHighlighter highlighter = tileToHighlight.GetComponentInChildren<TileHighlighter>();
        highlighter.SetEffected(true);
        highlighter.SetHighlighted(true);

        if (targetType != SquareTarget.UNDEFINED) {
            highlighter.SetTargetType(targetType);
        }
    }

    public void ClearEffectedTiles(bool clearSelectedUnit = false) {
        currentlyEffected.ForEach((tile) => {
            TileHighlighter highlighter = tile.GetComponentInChildren<TileHighlighter>();
            if (!clearSelectedUnit && highlighter.myTarget == SquareTarget.SELECTED_UNIT) {
                return;
            }

            highlighter.SetEffected(false);
            if (!currentlyHighlighted.Contains(tile)) {
                highlighter.CleanHighlight();
            }
            highlighter.ClearDecals();
        });
        currentlyEffected.Clear();
    }

    public void HighlightTiles(List<Node> tilesToHighlight, SquareTarget targetType) {
        foreach (Node n in tilesToHighlight) {
            currentlyHighlighted.Add(n);
            TileHighlighter highlighter = n.GetComponentInChildren<TileHighlighter>();
            highlighter.SetTargetType(targetType);
            highlighter.SetHighlighted(true);
        }
    }

    public void HighlightTile(Node tileToHighlight, SquareTarget targetType) {
        currentlyHighlighted.Add(tileToHighlight);
        tileToHighlight.GetComponentInChildren<TileHighlighter>().SetTargetType(targetType);
        tileToHighlight.GetComponentInChildren<TileHighlighter>().SetHighlighted(true);
    }

    public void UnhighlightTiles() {
        foreach (Node n in currentlyHighlighted) {
            UnhighlightTile(n);
        }
        currentlyHighlighted = new List<Node>();
    }

    public void UnhighlightAllTiles() {
        UnhighlightTiles();
        ClearEffectedTiles();
    }

    public void UnhighlightTile(Node tileToHighlight) {
        tileToHighlight.GetComponentInChildren<TileHighlighter>().CleanHighlight();
    }
}