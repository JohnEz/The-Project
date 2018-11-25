using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    CameraController camera;

	TileMap map;

	void Start () {

	}

	public void Initialise() {
		map = GetComponentInChildren<TileMap> ();
        camera = GameObject.Find("Main Camera").GetComponent<CameraController>();

        camera.Initialise (map);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void MoveToLocation(Vector2 location) {
		camera.MoveToTarget (location);
	}

	public void MoveToLocation(Node node) {
		MoveToLocation (map.getPositionOfNode(node));
	}
}
