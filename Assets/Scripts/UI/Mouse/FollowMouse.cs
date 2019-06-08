using UnityEngine;
using System.Collections;

public class FollowMouse : MonoBehaviour {

    public void Update() {
        transform.position = Input.mousePosition;
    }
}