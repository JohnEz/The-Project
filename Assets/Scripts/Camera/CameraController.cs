using UnityEngine;
using System.Collections;

public enum CameraMoveState {
    FREE,
    MOVING_TO_LOCATION,
    FOLLOWING_UNIT
}

public class CameraController : MonoBehaviour {

	const float MOVE_SPEED = 1200; //speed the camera moves
	const float ZOOM_SPEED = 0.1f; // speed the camera zooms
	const float ZOOM_CHANGE = 1f; //change in camera zoom
	const float ZOOM_CLOSE_ENOUGH = 0.02f; // how close the zoom needs to be before it snaps
	const float MINIMUM_ZOOM = 1f; //minimum zoom value
	const float MAXIMUM_ZOOM = 2f; //maximum zoom value
	const float BOUNDARY = -5; //distance from edge of screen that the camera starts to move
	const int TARGET_WIDTH = 1280;

	public float mapWidth = 0;
	public float mapHeight = 0;

	public CameraMoveState movementState = CameraMoveState.FREE;
	Vector3 targetLocation;
    Transform followTarget;

    public float minX = -100000;
	public float minY = -100000;
	public float maxX = 100000;
	public float maxY = 100000;

	float textureSize = 95;

	float currentZoom = 1f;
	float targetZoom = 1f;

	int pixelsToUnits = 1;

	bool mouseMovement = false;

	int height;

    Vector3 movement = new Vector3();



	void Awake() {
        Camera.main.orthographicSize = Screen.height / pixelsToUnits / 2f;
    }

    // Use this for initialization
    void Start () {

	}

	public void Initialise(TileMap map) {
		mapHeight = map.getHeight () * textureSize;
		mapWidth = map.getWidth () * textureSize;

		CalculateBounds ();
	}

	public void CalculateBounds() {
		float vertExtent = Camera.main.GetComponent<Camera>().orthographicSize;    
		float horzExtent = vertExtent * Screen.width / Screen.height;

		minX = RoundToNearestPixel(horzExtent - (0.5f * textureSize), GetComponent<Camera>());
		maxX = RoundToNearestPixel(mapHeight - horzExtent - (0.5f * textureSize), GetComponent<Camera>());
		minY = RoundToNearestPixel(vertExtent + (0.5f * textureSize) - mapWidth, GetComponent<Camera>());
		maxY = RoundToNearestPixel(-vertExtent + (0.5f * textureSize), GetComponent<Camera>());
	}

	// Update is called once per frame
	void Update () {
		UpdateMovement ();
        //UpdateZoom ();
        ClampBounds();
    }

    void LateUpdateLegacy() {
        Vector3 clampedMovement = new Vector3((int)movement.x, (int)movement.y);

        if (clampedMovement.magnitude >= 1.0f) {
            movement -= clampedMovement;
            if (clampedMovement != Vector3.zero) {
                transform.position = new Vector3((int)transform.position.x, (int)transform.position.y, (int)transform.position.z) + clampedMovement;
            }
        }

        
    }

	//updates the position of the camera via keyboard input
	void UpdateMovement() {

        // TODO make a switch statement
        if (movementState == CameraMoveState.FREE) {
            //allow user to move camera
            movement = new Vector3(0, 0, 0);


            if ((Input.GetKey(KeyCode.W) || (Input.mousePosition.y > Screen.height - BOUNDARY && mouseMovement))) {
                movement.y += 1;
            }
            if ((Input.GetKey(KeyCode.A) || (Input.mousePosition.x < BOUNDARY && mouseMovement))) {
                movement.x -= 1;
            }
            if ((Input.GetKey(KeyCode.S) || (Input.mousePosition.y < BOUNDARY && mouseMovement))) {
                movement.y -= 1;
            }
            if ((Input.GetKey(KeyCode.D) || (Input.mousePosition.x > Screen.width - BOUNDARY && mouseMovement))) {
                movement.x += 1;
            }

            movement.Normalize();
            Vector3 newPos = transform.position + (movement * MOVE_SPEED) * Time.deltaTime;
            Vector3 roundPos = new Vector3(RoundToNearestPixel(newPos.x, GetComponent<Camera>()), RoundToNearestPixel(newPos.y, GetComponent<Camera>()), newPos.z);

            transform.position = roundPos;
        } else if (movementState == CameraMoveState.FOLLOWING_UNIT || movementState == CameraMoveState.MOVING_TO_LOCATION) {
            if (movementState == CameraMoveState.MOVING_TO_LOCATION && Vector3.Distance(transform.position, targetLocation) < 2f) {
                movementState = CameraMoveState.FREE;
                return;
            }

            Vector3 moveLocation = movementState == CameraMoveState.MOVING_TO_LOCATION ? targetLocation : new Vector3(followTarget.position.x, followTarget.position.y, transform.position.z);

            //Smoothly animate towards the correct location
            transform.position = Vector3.Lerp(transform.position, moveLocation, 6f * Time.deltaTime);
        }

	}

	void UpdateZoom() {
		float d = Input.GetAxis("Mouse ScrollWheel");

		//float unitsPerPixel = 1f / textureSize;
		bool zoomHasChanged = false;

		Camera cam = GetComponent<Camera> ();

		if (d != 0) {
			int direction = -(int)Mathf.Sign (d);

			targetZoom += ZOOM_CHANGE * direction;

			targetZoom = Mathf.Clamp (targetZoom, MINIMUM_ZOOM, MAXIMUM_ZOOM);
		}

		if (Mathf.Abs (targetZoom - currentZoom) > ZOOM_CLOSE_ENOUGH) {
			zoomHasChanged = true;
			currentZoom = Mathf.Lerp (currentZoom, targetZoom, ZOOM_SPEED);
		} else if (currentZoom != targetZoom) {
			zoomHasChanged = true;
			currentZoom = targetZoom;
		}

		if (zoomHasChanged) {
			//cam.orthographicSize = ((Screen.height / 2f) * unitsPerPixel)*currentZoom;
			cam.orthographicSize = Screen.height / 2f * currentZoom;
			CalculateBounds ();
		}
	}


	public void MoveToTarget(Vector3 pos) {
		MoveToTarget (new Vector2 (pos.x, pos.y));
	}

	public void MoveToTarget(Vector2 pos) {
        Debug.Log("moving to " + pos);

        movementState = CameraMoveState.MOVING_TO_LOCATION;
        followTarget = null;
		Vector3 clampedTarget = GetClampedPosition (pos);
		//Vector3 clampedTarget = pos;
		targetLocation = new Vector3(RoundToNearestPixel(clampedTarget.x, GetComponent<Camera>()), RoundToNearestPixel(clampedTarget.y, GetComponent<Camera>()), transform.position.z);
	}

	public static float RoundToNearestPixel(float unityUnits, Camera viewingCamera)
	{
		float valueInPixels = (Screen.height / (viewingCamera.orthographicSize * 2)) * unityUnits;
		valueInPixels = Mathf.Round(valueInPixels);
		float adjustedUnityUnits = valueInPixels / (Screen.height / (viewingCamera.orthographicSize * 2));
		return adjustedUnityUnits;
	}

	public void ClampBounds() {
		transform.position = GetClampedPosition(transform.position);
	}

	public Vector3 GetClampedPosition(Vector3 targetPosition) {
		Vector3 clampedPosition = targetPosition;
		clampedPosition.x = Mathf.Clamp (clampedPosition.x, minX, maxX);
		clampedPosition.y = Mathf.Clamp (clampedPosition.y, minY, maxY);
		return clampedPosition;
	}

    public void FollowTarget(Transform target) {
        movementState = CameraMoveState.FOLLOWING_UNIT;
        followTarget = target;
    }
}
