using UnityEngine;

public class SlidingMenu : MonoBehaviour {
    private float DEFAULT_SLIDE_SPEED = 8f;
    private float MIN_DISTANCE = 0.05f;

    private float slidingSpeed;
    private float time = 0;
    private Vector3 preSlideLocation;
    private bool isSliding = false;
    private Vector3 targetPosition;

    private Vector3 startingPosition;
    public Vector3 defaultOutPosition;

    private RectTransform rectTransform;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        startingPosition = rectTransform.anchoredPosition;
        targetPosition = startingPosition;
    }

    // Update is called once per frame
    private void Update() {
        if (targetPosition == null || slidingSpeed <= 0) {
            return;
        }

        float distanceToNode = Vector3.Distance(targetPosition, transform.localPosition);

        if (distanceToNode > MIN_DISTANCE) {
            rectTransform.anchoredPosition = Vector3.Lerp(preSlideLocation, targetPosition, time);
            time += Time.deltaTime / slidingSpeed;
        } else {
            rectTransform.anchoredPosition = targetPosition;
        }
    }

    public void OpenMenu(float speed = 0) {
        slidingSpeed = speed != 0 ? speed : DEFAULT_SLIDE_SPEED;
        time = 0;
        targetPosition = startingPosition;
        preSlideLocation = rectTransform.anchoredPosition;
    }

    public void CloseMenu(float speed = 0) {
        slidingSpeed = speed != 0 ? speed : DEFAULT_SLIDE_SPEED;
        time = 0;
        targetPosition = defaultOutPosition;
        preSlideLocation = rectTransform.anchoredPosition;
    }

    public void SlideToPosition(Vector3 _targetPosition) {
        targetPosition = _targetPosition;
    }
}