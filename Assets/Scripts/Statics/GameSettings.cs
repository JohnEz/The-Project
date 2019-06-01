using UnityEngine;
using System.Collections;

public static class GameSettings {
    private static bool mouseCanMoveCamera = true;

    public static bool MouseCanMoveCamera {
        get {
            return mouseCanMoveCamera;
        }
        set {
            mouseCanMoveCamera = value;
        }
    }
}