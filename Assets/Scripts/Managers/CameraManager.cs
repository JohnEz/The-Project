using UnityEngine;

public class CameraManager : MonoBehaviour {
    public static CameraManager singleton;

    public CameraController cam;

    private TileMap map;

    private void Awake() {
        singleton = this;
    }

    private void Start() {
    }

    public void Initialise() {
        map = GetComponentInChildren<TileMap>();

        cam.Initialise(map);
    }

    // Update is called once per frame
    private void Update() {
    }

    public void MoveToLocation(Vector2 location) {
        cam.MoveToTarget(location);
    }

    public void MoveToLocation(Node node) {
        MoveToLocation(map.getPositionOfNode(node));
    }

    public void JumpToLocation(Node node) {
        cam.JumpToLocation(map.getPositionOfNode(node));
    }

    public void FollowTarget(Transform target) {
        cam.FollowTarget(target);
    }
}