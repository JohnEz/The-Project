using Cinemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct CameraAndOrientation {
    public CinemachineVirtualCamera camera;
    public Vector3 forwardDir;
    public Vector3 rightDir;
}

public class CameraController3D : MonoBehaviour {
    private const float MOVE_SPEED = 120; //speed the camera moves
    private const float ZOOM_SPEED = 0.1f; // speed the camera zooms
    private const float ZOOM_CHANGE = 1f; //change in camera zoom
    private const float ZOOM_CLOSE_ENOUGH = 0.02f; // how close the zoom needs to be before it snaps
    private const float MINIMUM_ZOOM = 1f; //minimum zoom value
    private const float MAXIMUM_ZOOM = 2f; //maximum zoom value
    private const float BOUNDARY = -5; //distance from edge of screen that the camera starts to move
    private const int TARGET_WIDTH = 1280;

    [HideInInspector]
    public float mapWidth = 0;

    [HideInInspector]
    public float mapHeight = 0;

    private bool isControllable = true;
    private Vector3 targetLocation;

    [HideInInspector]
    public float minX = -100000;

    [HideInInspector]
    public float minZ = -100000;

    [HideInInspector]
    public float maxX = 100000;

    [HideInInspector]
    public float maxZ = 100000;

    private float textureSize = 12.8f;

    private float currentZoom = 1f;
    private float targetZoom = 1f;

    private Vector3 movement = new Vector3();

    public List<CameraAndOrientation> cameraAngles;
    public int currentCameraIndex = 0;

    public CinemachineVirtualCamera activeCam;

    private void Awake() {
    }

    // Use this for initialization
    private void Start() {
        activeCam = cameraAngles[currentCameraIndex].camera;
    }

    public void Initialise() {
        mapHeight = TileMap.instance.getHeight() * textureSize;
        mapWidth = TileMap.instance.getWidth() * textureSize;
        CalculateBounds();
    }

    public void TurnOff() {
        isControllable = false;
        activeCam.GetComponent<CinemachineVirtualCamera>().Priority = 1;
    }

    public void TurnOn() {
        isControllable = true;
        activeCam.GetComponent<CinemachineVirtualCamera>().Priority = 10;
    }

    public void CalculateBounds() {
        float halfTextureSize = textureSize / 2;

        minX = -halfTextureSize;
        minZ = -mapHeight + halfTextureSize;

        maxX = mapWidth - halfTextureSize;
        maxZ = +halfTextureSize;
    }

    // Update is called once per frame
    private void Update() {
        UpdateMovement();
        //UpdateZoom ();
        ClampBounds();
    }

    //updates the position of the camera via keyboard input
    private void UpdateMovement() {
        // TODO make a switch statement
        if (isControllable) {
            //allow user to move camera
            movement = new Vector3(0, 0, 0);

            // TODO calculate this correctly
            Vector3 forward = cameraAngles[currentCameraIndex].forwardDir;
            Vector3 right = cameraAngles[currentCameraIndex].rightDir;

            if ((Input.GetKey(KeyCode.W) || Input.GetKey("up") || (Input.mousePosition.y > Screen.height - BOUNDARY && GameSettings.MouseCanMoveCamera))) {
                movement += forward;
            }
            if ((Input.GetKey(KeyCode.A) || Input.GetKey("left") || (Input.mousePosition.x < BOUNDARY && GameSettings.MouseCanMoveCamera))) {
                movement -= right;
            }
            if ((Input.GetKey(KeyCode.S) || Input.GetKey("down") || (Input.mousePosition.y < BOUNDARY && GameSettings.MouseCanMoveCamera))) {
                movement -= forward;
            }
            if ((Input.GetKey(KeyCode.D) || Input.GetKey("right") || (Input.mousePosition.x > Screen.width - BOUNDARY && GameSettings.MouseCanMoveCamera))) {
                movement += right;
            }

            if (Input.GetKeyDown(KeyCode.Q)) {
                TurnOff();

                currentCameraIndex++;
                currentCameraIndex = currentCameraIndex % cameraAngles.Count;

                activeCam = cameraAngles[currentCameraIndex].camera;

                TurnOn();
            }
            if (Input.GetKeyDown(KeyCode.E)) {
                TurnOff();

                currentCameraIndex--;
                if (currentCameraIndex < 0) {
                    currentCameraIndex = cameraAngles.Count - 1;
                }

                activeCam = cameraAngles[currentCameraIndex].camera;

                TurnOn();
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
            //CalculateBounds();
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
        location.y = 0;
        transform.position = location;
    }

    public Vector3 GetClampedPosition(Vector3 targetPosition) {
        Vector3 clampedPosition = targetPosition;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, minZ, maxZ);
        return clampedPosition;
    }

    public void ClampBounds() {
        transform.position = GetClampedPosition(transform.position);
    }

    public void FollowTarget(Transform target) {
        //movementState = CameraMoveState.FOLLOWING_UNIT;
        //followTarget = target;
    }
}