using UnityEngine;
using UnityEngine.UI;

public class SlidingMenu : MonoBehaviour {
    private float DEFAULT_SLIDE_SPEED = 8f;
    private float MIN_DISTANCE = 0.05f;

    private Vector3 targetPosition;

    private Vector3 startingPosition;
    private Vector2 targetScreenSize;
    public Vector2 anchorPosition;

    public bool startsClosed = false;

    private RectTransform rectTransform;

    public void Start() {
        rectTransform = GetComponent<RectTransform>();
        startingPosition = rectTransform.anchoredPosition;
        targetPosition = startingPosition;
        targetScreenSize = GameObject.Find("MenuCanvas").GetComponent<CanvasScaler>().referenceResolution;
    }

    // Update is called once per frame
    private void Update() {
        if (targetPosition == null) {
            return;
        }

        float distanceToNode = Vector3.Distance(targetPosition, transform.localPosition);

        if (distanceToNode > MIN_DISTANCE) {
            rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, targetPosition, DEFAULT_SLIDE_SPEED * Time.deltaTime);
        } else {
            rectTransform.anchoredPosition = targetPosition;
        }
    }

    public void OpenMenu() {
        Vector3 openPosition = new Vector3((targetScreenSize.x / 2), (targetScreenSize.y / 2)) * -anchorPosition;

        SlideToPosition(openPosition);
    }

    public void CloseMenu() {
        if (startsClosed) {
            SlideToPosition(startingPosition);
        } else {
            Debug.Log("Tried to close menu without close location");
        }
    }

    public void CloseMenu(Vector2 direction) {
        Vector3 closedPosition = new Vector3((targetScreenSize.x), (targetScreenSize.y)) * direction;
        SlideToPosition(closedPosition);
    }

    public void SlideToPosition(Vector3 position) {
        targetPosition = position;
    }
}