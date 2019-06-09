using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    private const float DRAG_SPEED = 25F;
    private const float MIN_DISTANCE_TO_LOCATION = 1f;

    private Vector3 targetLocation;

    public Transform originalParent;
    public Transform placeholderParent;

    private GameObject placeholder = null;

    public bool droppedOnZone;
    private bool beingDragged = false;

    public Dropzone currentZone;

    private void Start() {
        originalParent = transform.parent;
    }

    private void Update() {
        // move to target location
        if (beingDragged && Vector3.Distance(targetLocation, transform.position) > MIN_DISTANCE_TO_LOCATION) {
            this.transform.position = Vector3.Lerp(transform.position, targetLocation, DRAG_SPEED * Time.deltaTime);
        }
    }

    public void SetDragged(bool isBeingDragged) {
        beingDragged = isBeingDragged;
    }

    public void SetTargetPosition(Vector3 target) {
        targetLocation = new Vector3(target.x, target.y - (GetComponent<RectTransform>().rect.height / 4), target.z);
    }

    public void OnBeginDrag(PointerEventData eventData) {
        CreatePlaceholder();

        SetDragged(true);
        originalParent = transform.parent;
        transform.SetParent(GameObject.Find("Canvas").transform);

        SetTargetPosition(eventData.position);

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) {
        SetTargetPosition(eventData.position);

        //AdjustPlaceHolder();
    }

    public void OnDestroy() {
        Destroy(placeholder);
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (!beingDragged) {
            return;
        }

        SetDragged(false);
        ReturnToParent(eventData.position);

        droppedOnZone = false;

        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    // Creates a placeholder where this card was in the hand
    private void CreatePlaceholder() {
        RectTransform myRect = GetComponent<RectTransform>();

        placeholderParent = originalParent;

        placeholder = new GameObject();
        placeholder.transform.SetParent(placeholderParent);

        LayoutElement le = placeholder.AddComponent<LayoutElement>();
        le.preferredWidth = GetComponent<LayoutElement>().preferredWidth;
        le.preferredHeight = GetComponent<LayoutElement>().preferredHeight;
        le.flexibleHeight = 0;
        le.flexibleWidth = 0;

        placeholder.transform.SetSiblingIndex(transform.GetSiblingIndex());
    }

    private void AdjustPlaceHolder() {
        int newSiblingIndex = placeholderParent.childCount;

        if (placeholder.transform.parent != placeholderParent) {
            placeholder.transform.SetParent(placeholderParent);
        }

        for (int i = 0; i < placeholderParent.childCount; i++) {
            if (transform.position.x < placeholderParent.GetChild(i).position.x) {
                newSiblingIndex = i;

                if (placeholder.transform.GetSiblingIndex() < newSiblingIndex) {
                    newSiblingIndex--;
                }
            }
        }

        placeholder.transform.SetSiblingIndex(newSiblingIndex);
    }

    public void SetNewParent(Dropzone newZone) {
        if (currentZone) {
            currentZone.RemoveDraggedItem(gameObject);
        }
        currentZone = newZone;
        originalParent = newZone.ZoneTransform;
        droppedOnZone = true;
    }

    public void ReturnToParent(Vector2 position) {
        SetTargetPosition(position);

        // Jump back into the hand
        transform.SetParent(originalParent);

        // Place card at correct index in hand
        transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());

        Destroy(placeholder);
    }
}