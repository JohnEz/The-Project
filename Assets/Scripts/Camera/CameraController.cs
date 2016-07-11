using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	const float MOVESPEED = 12; //speed the camera moves
	const float ZOOMSPEED = 0.2f; //speed the camera zooms
	const float MINZOOM = 1f; //minimum zoom value
	const float MAXZOOM = 2f; //maximum zoom value
	const float BOUNDARY = -5; //distance from edge of screen that the camera starts to move

	public float screenWidth = 0;
	public float screenHeight = 0;

	public bool movingToDestination = false;
	Vector3 targetLocation;

	public float minX = -100000;
	public float minY = -100000;
	public float maxX = 100000;
	public float maxY = 100000;

	public float TestX = 0;
	public float TestY = 0;

	public float textureSize = 128f;

	float currentZoom = 1f;

	// Use this for initialization
	void Start () {
		float unitsPerPixel = 1f / textureSize;

		Camera.main.orthographicSize = ((Screen.height / 2f) * unitsPerPixel)*currentZoom;
	}

	public void Initialise(TileMap map) {
		minX = -100000;
		minY = -100000;

		maxX = 100000;
		maxY = 100000;
	}

	// Update is called once per frame
	void Update () {
		UpdateMovement ();
		UpdateZoom ();
	}

	//updates the position of the camera via keyboard input
	void UpdateMovement() {

		TestX = transform.position.x;
		TestY = transform.position.y;



		//if the camera is moving to a destination
		if (movingToDestination) {

			if (Vector3.Distance (transform.position, targetLocation) < 0.2f) {
				movingToDestination = false;
			}
			//Smoothly animate towards the correct location
			transform.position = Vector3.Lerp (transform.position, targetLocation, 6f * Time.deltaTime);
		} else {
			//allow user to move camera
			Vector3 move = new Vector3 (0, 0, 0);


			if ((Input.GetKey(KeyCode.W) || Input.mousePosition.y > Screen.height - BOUNDARY) && transform.position.y < maxY) {
				move += new Vector3(0, 1, 0);
			}
			if ((Input.GetKey(KeyCode.A) || Input.mousePosition.x < BOUNDARY) && transform.position.x > minX) {
				move -= new Vector3(1, 0, 0);
			}
			if ((Input.GetKey(KeyCode.S) || Input.mousePosition.y < BOUNDARY) && transform.position.y > minY) {
				move -= new Vector3(0, 1, 0);
			}
			if ((Input.GetKey(KeyCode.D) || Input.mousePosition.x > Screen.width - BOUNDARY) && transform.position.x < maxX) {
				move += new Vector3(1, 0, 0);
			}

			move.Normalize ();
			transform.position += (move * MOVESPEED) * Time.deltaTime;
		}
	}

	void UpdateZoom() {
		float d = Input.GetAxis("Mouse ScrollWheel");

		float unitsPerPixel = 1f / textureSize;

		Camera cam = GetComponent<Camera> ();

		if (d > 0f)
		{
			//zoom in
			currentZoom -= ZOOMSPEED;

			if (currentZoom < MINZOOM) {
				currentZoom = MINZOOM;
			}
		}
		else if (d < 0f)
		{
			//zoom out
			currentZoom += ZOOMSPEED;

			if (currentZoom > MAXZOOM) {
				currentZoom = MAXZOOM;
			}
		}

		cam.orthographicSize = ((Screen.height / 2f) * unitsPerPixel)*currentZoom;

	}


	public void MoveToTarget(Vector3 pos) {
		movingToDestination = true;
		targetLocation = new Vector3 (pos.x, pos.y, transform.position.z);
	}
}
