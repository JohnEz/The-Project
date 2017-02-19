using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	const float MOVESPEED = 12; //speed the camera moves
	const float ZOOMSPEED = 1f; //speed the camera zooms
	const float MINZOOM = 1f; //minimum zoom value
	const float MAXZOOM = 3f; //maximum zoom value
	const float BOUNDARY = -5; //distance from edge of screen that the camera starts to move

	public float mapWidth = 0;
	public float mapHeight = 0;

	public bool movingToDestination = false;
	Vector3 targetLocation;

	public float minX = -100000;
	public float minY = -100000;
	public float maxX = 100000;
	public float maxY = 100000;

	public float textureSize = 128f;
	float unitsPerPixel;

	float currentZoom = 1f;

	public bool mouseMovement = false;

	// Use this for initialization
	void Start () {


	}

	public void Initialise(TileMap map) {

		unitsPerPixel = 1f / textureSize;

		Camera.main.orthographicSize = ((Screen.height / 2f) * unitsPerPixel)*currentZoom;

		mapHeight = map.getHeight ();
		mapWidth = map.getWidth ();

		CalculateBounds ();
	}

	public void CalculateBounds() {
		float vertExtent = Camera.main.GetComponent<Camera>().orthographicSize;    
		float horzExtent = vertExtent * Screen.width / Screen.height;

		minX = horzExtent - 0.5f;
		maxX = mapHeight - horzExtent - 0.5f;
		minY = vertExtent + 0.5f - mapWidth;
		maxY = -vertExtent + 0.5f;
	}

	// Update is called once per frame
	void Update () {
		UpdateMovement ();
		UpdateZoom ();
		ClampBounds ();
	}

	//updates the position of the camera via keyboard input
	void UpdateMovement() {

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


			if ((Input.GetKey(KeyCode.W) || (Input.mousePosition.y > Screen.height - BOUNDARY && mouseMovement))) {
				move += new Vector3(0, 1, 0);
			}
			if ((Input.GetKey(KeyCode.A) || (Input.mousePosition.x < BOUNDARY && mouseMovement))) {
				move -= new Vector3(1, 0, 0);
			}
			if ((Input.GetKey(KeyCode.S) || (Input.mousePosition.y < BOUNDARY && mouseMovement))) {
				move -= new Vector3(0, 1, 0);
			}
			if ((Input.GetKey(KeyCode.D) || (Input.mousePosition.x > Screen.width - BOUNDARY && mouseMovement))) {
				move += new Vector3(1, 0, 0);
			}

			move.Normalize ();
			Vector3 newPos = transform.position + (move * MOVESPEED) * Time.deltaTime;
			Vector3 roundPos = new Vector3(RoundToNearestPixel(newPos.x, GetComponent<Camera>()), RoundToNearestPixel(newPos.y, GetComponent<Camera>()), newPos.z);

			transform.position = roundPos;
		}
	}

	void UpdateZoom() {
		float d = Input.GetAxis("Mouse ScrollWheel");

		float unitsPerPixel = 1f / textureSize;

		Camera cam = GetComponent<Camera> ();

		if (d != 0) {
			if (d > 0f) {
				//zoom in
				currentZoom -= ZOOMSPEED;

				if (currentZoom < MINZOOM) {
					currentZoom = MINZOOM;
				}
			} else if (d < 0f) {
				//zoom out
				currentZoom += ZOOMSPEED;

				if (currentZoom > MAXZOOM) {
					currentZoom = MAXZOOM;
				}
			}
			cam.orthographicSize = ((Screen.height / 2f) * unitsPerPixel)*currentZoom;
			CalculateBounds ();
		}
	}


	public void MoveToTarget(Vector3 pos) {
		movingToDestination = true;
		targetLocation = new Vector3 (pos.x, pos.y, transform.position.z);
	}

	public static float RoundToNearestPixel(float unityUnits, Camera viewingCamera)
	{
		float valueInPixels = (Screen.height / (viewingCamera.orthographicSize * 2)) * unityUnits;
		valueInPixels = Mathf.Round(valueInPixels);
		float adjustedUnityUnits = valueInPixels / (Screen.height / (viewingCamera.orthographicSize * 2));
		return adjustedUnityUnits;
	}

	public void ClampBounds() {
		Vector3 clampedPosition = transform.position;
		clampedPosition.x = Mathf.Clamp (clampedPosition.x, minX, maxX);
		clampedPosition.y = Mathf.Clamp (clampedPosition.y, minY, maxY);
		transform.position = clampedPosition;
	}
}
