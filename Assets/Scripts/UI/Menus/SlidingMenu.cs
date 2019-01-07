using UnityEngine;

public class SlidingMenu : MonoBehaviour {
    private float SLIDE_SPEED = 8f;
    private float MIN_DISTANCE = 0.05f;

    private Vector3 startingPosition;
    private bool isSliding = false;
    private Vector3 targetPosition;

    private void Awake() {
        startingPosition = transform.localPosition;
        targetPosition = startingPosition;
    }

    // Update is called once per frame
    private void Update() {
        if (targetPosition == null || SLIDE_SPEED <= 0) {
            return;
        }

        float distanceToNode = Vector3.Distance(targetPosition, transform.localPosition);

        if (distanceToNode > MIN_DISTANCE) {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, SLIDE_SPEED * Time.deltaTime);
        } else {
            transform.localPosition = targetPosition;
        }
    }

    public void CloseMenu() {
        targetPosition = startingPosition;
    }

    public void SlideToPosition(Vector3 _targetPosition) {
        targetPosition = _targetPosition;
    }
}