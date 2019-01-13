using UnityEngine;

public class CameraManager : MonoBehaviour {
    public static CameraManager singleton;

    public CameraController3D cam;

    private void Awake() {
        singleton = this;
    }

    private void Start() {
    }

    public void Initialise() {

    }

    // Update is called once per frame
    private void Update() {
    }

    public void MoveToLocation(Vector2 location) {
        cam.MoveToTarget(location);
    }

    public void MoveToLocation(Node node) {
        MoveToLocation(TileMap.instance.getPositionOfNode(node));
    }

    public void JumpToLocation(Node node) {
        cam.JumpToLocation(TileMap.instance.getPositionOfNode(node));
    }

    public void FollowTarget(Transform target) {
        cam.FollowTarget(target);
    }
}