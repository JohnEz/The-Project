using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    public static CameraManager singleton;

    public Camera physicalCamera;
    public CameraController3D controlledCamera;
    public GameObject followCameraPrefab;
    public GameObject personalCamera;

    [HideInInspector]
    public CinemachineVirtualCamera activeFollowCamera;

    Vector3 cameraOffset;

    private void Awake() {
        singleton = this;
        
    }

    private void Start() {
        
    }

    public void Initialise() {
        cameraOffset = followCameraPrefab.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        controlledCamera.Initialise(cameraOffset);
    }

    // Update is called once per frame
    private void Update() {
    }

    public void MoveToLocation(Vector2 location) {
        TurnOffCameras();
        controlledCamera.TurnOn();
        controlledCamera.MoveToTarget(location);
    }

    public void MoveToLocation(Node node) {
        MoveToLocation(TileMap.instance.getPositionOfNode(node));
    }

    public void JumpToLocation(Node node) {
        TurnOffCameras();
        controlledCamera.TurnOn();
        controlledCamera.JumpToLocation(TileMap.instance.getPositionOfNode(node) + cameraOffset);
    }

    public void FollowTarget(Transform target) {
        TurnOffCameras();
        GameObject followCamera = Instantiate(followCameraPrefab);
        CinemachineVirtualCamera newFollowCamera = followCamera.GetComponent<CinemachineVirtualCamera>();

        newFollowCamera.Follow = target;
        newFollowCamera.Priority = 10;

        if (activeFollowCamera != null) {
            Destroy(activeFollowCamera.gameObject, 2.1f);
        }
        activeFollowCamera = newFollowCamera;
    }

    public void TurnOffCameras() {
        controlledCamera.TurnOff();

        if (activeFollowCamera != null) {
            activeFollowCamera.Priority = 0;
        }
    }
}