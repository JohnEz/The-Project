using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public static CameraManager singleton;

	public CameraController cam;

	TileMap map;

    private void Awake() {
        singleton = this;
    }

    void Start () {

	}

	public void Initialise() {
		map = GetComponentInChildren<TileMap> ();

		cam.Initialise (map);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void MoveToLocation(Vector2 location) {
		cam.MoveToTarget (location);
	}

	public void MoveToLocation(Node node) {
		MoveToLocation (map.getPositionOfNode(node));
	}

    public void FollowTarget(Transform target) {
        cam.FollowTarget(target);
    }
}
