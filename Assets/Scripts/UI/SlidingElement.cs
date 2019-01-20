using UnityEngine;

public class SlidingElement : MonoBehaviour {
    private float DEFAULT_SLIDE_SPEED = 8f;
    private float DEFAULT_SLIDE_TIME = 1f;
    private float MIN_DISTANCE = 0.05f;

    private float timeToDestination;
    private float time = 0;
    private Vector3 preSlideLocation;
    private Vector3 targetPosition;
    private bool setTime = false;

    private Vector3 startingPosition;
    public Vector3 defaultOutPosition;

    private RectTransform rectTransform;

    public bool startedClosed = false;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        startingPosition = rectTransform.anchoredPosition;
        targetPosition = startingPosition;
    }

    // Update is called once per frame
    private void Update() {
        if (targetPosition == Vector3.zero || targetPosition == transform.localPosition) {
            return;
        }

        float distanceToNode = Vector3.Distance(targetPosition, transform.localPosition);

        if (distanceToNode > MIN_DISTANCE) {
            if (setTime) {
                rectTransform.anchoredPosition = Vector3.Lerp(preSlideLocation, targetPosition, time);
                time += Time.deltaTime / timeToDestination;
            } else {
                rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, targetPosition, Time.deltaTime * DEFAULT_SLIDE_SPEED);
            }
        } else {
            rectTransform.anchoredPosition = targetPosition;
        }
    }

    public void OpenMenu() {
        setTime = false;
        MoveToLocation(startedClosed ? defaultOutPosition : startingPosition);
    }

    public void CloseMenu() {
        setTime = false;
        MoveToLocation(startedClosed ? startingPosition : defaultOutPosition);
    }

    public void OpenMenu(float time) {
        setTime = true;
        timeToDestination = time != 0 ? time : DEFAULT_SLIDE_TIME;
        MoveToLocation(startedClosed ? defaultOutPosition : startingPosition);
    }

    public void CloseMenu(float time) {
        setTime = true;
        timeToDestination = time != 0 ? time : DEFAULT_SLIDE_TIME;
        MoveToLocation(startedClosed ? startingPosition : defaultOutPosition);
    }

    public void MoveToLocation(Vector3 position) {
        time = 0;
        targetPosition = position;
        preSlideLocation = rectTransform.anchoredPosition;
    }

    public void SlideToPosition(Vector3 _targetPosition) {
        targetPosition = _targetPosition;
    }
}