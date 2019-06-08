using UnityEngine;
using System.Collections;
using DuloGames.UI;

public class MouseTooltip : MonoBehaviour {
    public static MouseTooltip instance;

    public void Awake() {
        instance = this;
    }
}