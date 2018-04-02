using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    const float DRAG_SPEED = 25F;
    const float MIN_DISTANCE = 1f;

    Vector3 targetLocation;

    public Transform originalParent;
    public Transform placeholderParent;

    GameObject placeholder = null;

    bool beingDragged = false;

    void Start() {
        
    }

    void Update() {
        if (beingDragged && Vector3.Distance(targetLocation, transform.position) > MIN_DISTANCE) {
            this.transform.position = Vector3.Lerp(transform.position, targetLocation, DRAG_SPEED * Time.deltaTime);
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        beingDragged = true;
        originalParent = transform.parent;
        
        CreatePlaceholder();
        
        transform.SetParent(GameObject.Find("Canvas").transform);

        targetLocation = eventData.position;

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) {
        targetLocation = eventData.position;

        AdjustPlaceHolder();
    }

    public void OnEndDrag(PointerEventData eventData) {
        beingDragged = false;
        targetLocation = eventData.position;
        transform.SetParent(originalParent);
        transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());

        Destroy(placeholder);

        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    void CreatePlaceholder() {
        RectTransform myRect = GetComponent<RectTransform>();

        placeholderParent = originalParent;

        placeholder = new GameObject();
        placeholder.transform.SetParent(placeholderParent);
        RectTransform rect = placeholder.AddComponent<RectTransform>();
        rect.sizeDelta = myRect.sizeDelta;

        LayoutElement le = placeholder.AddComponent<LayoutElement>();
        le.preferredWidth = GetComponent<LayoutElement>().preferredWidth;
        le.preferredHeight = GetComponent<LayoutElement>().preferredHeight;
        le.flexibleHeight = 0;
        le.flexibleWidth = 0;

        placeholder.transform.SetSiblingIndex(transform.GetSiblingIndex());
    }

    void AdjustPlaceHolder() {
        int newSiblingIndex = placeholderParent.childCount;

        if (placeholder.transform.parent != placeholderParent) {
            placeholder.transform.SetParent(placeholderParent);
        }

        for (int i = 0; i < placeholderParent.childCount; i++) {
            if (transform.position.y > placeholderParent.GetChild(i).position.y) {
                newSiblingIndex = i;

                if (placeholder.transform.GetSiblingIndex() < newSiblingIndex) {
                    newSiblingIndex--;
                }
                
                break;
            }
        }

        placeholder.transform.SetSiblingIndex(newSiblingIndex);
    }
}
