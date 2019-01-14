using Cinemachine;
using UnityEngine;

public class CameraController3D : MonoBehaviour {
    private const float MOVE_SPEED = 1200; //speed the camera moves
    private const float ZOOM_SPEED = 0.1f; // speed the camera zooms
    private const float ZOOM_CHANGE = 1f; //change in camera zoom
    private const float ZOOM_CLOSE_ENOUGH = 0.02f; // how close the zoom needs to be before it snaps
    private const float MINIMUM_ZOOM = 1f; //minimum zoom value
    private const float MAXIMUM_ZOOM = 2f; //maximum zoom value
    private const float BOUNDARY = -5; //distance from edge of screen that the camera starts to move
    private const int TARGET_WIDTH = 1280;

    public float mapWidth = 0;
    public float mapHeight = 0;

    private bool isControllable = true;
    private Vector3 targetLocation;

    [HideInInspector]
    public float minX = -100000;

    [HideInInspector]
    public float minY = -100000;

    [HideInInspector]
    public float maxX = 100000;

    [HideInInspector]
    public float maxY = 100000;

    private float textureSize = 128;

    private float currentZoom = 1f;
    private float targetZoom = 1f;

    private int cardBuffer = 384; // how much the cards take up at the bottom of the screen

    private bool mouseMovement = false;

    private int height;

    private Vector3 movement = new Vector3();

    private void Awake() {

    }

    // Use this for initialization
    private void Start() {
    }

    public void Initialise() {

    }

    public void TurnOff() {
        isControllable = false;
        GetComponent<CinemachineVirtualCamera>().Priority = 0;
    }

    public void TurnOn() {
        isControllable = true;
        GetComponent<CinemachineVirtualCamera>().Priority = 10;
    }

    public void CalculateBounds() {
        //float vertExtent = Camera.main.GetComponent<Camera>().orthographicSize;
        //float horzExtent = vertExtent * Screen.width / Screen.height;

        //minX = RoundToNearestPixel(horzExtent - (0.5f * textureSize), GetComponent<Camera>());
        //maxX = RoundToNearestPixel(mapWidth - horzExtent - (0.5f * textureSize), GetComponent<Camera>());
        //minY = RoundToNearestPixel(vertExtent + (0.5f * textureSize) - mapHeight - cardBuffer, GetComponent<Camera>());
        //maxY = RoundToNearestPixel(-vertExtent + (0.5f * textureSize), GetComponent<Camera>());
    }

    // Update is called once per frame
    private void Update() {
        UpdateMovement();
        //UpdateZoom ();
    }

    //updates the position of the camera via keyboard input
    private void UpdateMovement() {
        // TODO make a switch statement
        if (isControllable) {
            //allow user to move camera
            movement = new Vector3(0, 0, 0);

            // TODO calculate this correctly
            Vector3 forward = new Vector3(1, 0, 1);
            Vector3 right = new Vector3(1, 0, -1);

            if ((Input.GetKey(KeyCode.W) || (Input.mousePosition.y > Screen.height - BOUNDARY && mouseMovement))) {
                movement += forward;
            }
            if ((Input.GetKey(KeyCode.A) || (Input.mousePosition.x < BOUNDARY && mouseMovement))) {
                movement -= right;
            }
            if ((Input.GetKey(KeyCode.S) || (Input.mousePosition.y < BOUNDARY && mouseMovement))) {
                movement -= forward;
            }
            if ((Input.GetKey(KeyCode.D) || (Input.mousePosition.x > Screen.width - BOUNDARY && mouseMovement))) {
                movement += right;
            }

            movement.Normalize();
            Vector3 newPos = transform.position + (movement * MOVE_SPEED) * Time.deltaTime;
            Vector3 roundPos = new Vector3(newPos.x, newPos.y, newPos.z);

            transform.position = roundPos;
        }
    }

    private void UpdateZoom() {
        float d = Input.GetAxis("Mouse ScrollWheel");

        //float unitsPerPixel = 1f / textureSize;
        bool zoomHasChanged = false;

        Camera cam = GetComponent<Camera>();

        if (d != 0) {
            int direction = -(int)Mathf.Sign(d);

            targetZoom += ZOOM_CHANGE * direction;

            targetZoom = Mathf.Clamp(targetZoom, MINIMUM_ZOOM, MAXIMUM_ZOOM);
        }

        if (Mathf.Abs(targetZoom - currentZoom) > ZOOM_CLOSE_ENOUGH) {
            zoomHasChanged = true;
            currentZoom = Mathf.Lerp(currentZoom, targetZoom, ZOOM_SPEED);
        } else if (currentZoom != targetZoom) {
            zoomHasChanged = true;
            currentZoom = targetZoom;
        }

        if (zoomHasChanged) {
            //cam.orthographicSize = ((Screen.height / 2f) * unitsPerPixel)*currentZoom;
            cam.orthographicSize = Screen.height / 2f * currentZoom;
            CalculateBounds();
        }
    }

    public void MoveToTarget(Vector3 pos) {
        //MoveToTarget(new Vector2(pos.x, pos.y));
    }

    public void MoveToTarget(Vector2 pos) {
        //movementState = CameraMoveState.MOVING_TO_LOCATION;
        //followTarget = null;
        //Vector3 clampedTarget = GetClampedPosition(pos);
        ////Vector3 clampedTarget = pos;
        //targetLocation = new Vector3(RoundToNearestPixel(clampedTarget.x, GetComponent<Camera>()), RoundToNearestPixel(clampedTarget.y, GetComponent<Camera>()), transform.position.z);
    }

    public void JumpToLocation(Vector3 location) {
        transform.position = location;
    }

    public void ClampBounds() {
        //transform.position = GetClampedPosition(transform.position);
    }

    public void FollowTarget(Transform target) {
        //movementState = CameraMoveState.FOLLOWING_UNIT;
        //followTarget = target;
    }
}