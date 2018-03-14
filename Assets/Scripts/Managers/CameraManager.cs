using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

	public CameraController cam;

	TileMap map;

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
}
