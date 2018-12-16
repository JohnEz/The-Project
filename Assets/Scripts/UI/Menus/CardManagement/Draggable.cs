﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler {

    const float DRAG_SPEED = 25F;
    const float MIN_DISTANCE_TO_LOCATION = 1f;
    const float SCALE_SPEED = 15f;
    const float MIN_DISTANCE_TO_SCALE = 0.05f;

    Vector3 targetLocation;
    Vector3 targetScale;

    public Transform originalParent;
    public Transform placeholderParent;

    GameObject placeholder = null;

    bool beingDragged = false;
    public bool droppedOnZone = false;

    //public bool isScaling;

    void Start() {
        targetScale = GetComponent<RectTransform>().localScale;
    }

    void Update() {
        // move to target location
        if (beingDragged && Vector3.Distance(targetLocation, transform.position) > MIN_DISTANCE_TO_LOCATION) {
            this.transform.position = Vector3.Lerp(transform.position, targetLocation, DRAG_SPEED * Time.deltaTime);
        }

        // scale to target scale
        Vector3 currentScale = GetComponent<RectTransform>().localScale;
        if (Vector3.Distance(targetScale, currentScale) > MIN_DISTANCE_TO_SCALE) {
            GetComponent<RectTransform>().localScale = Vector3.Lerp(currentScale, targetScale, SCALE_SPEED * Time.deltaTime);
        } else if (currentScale != targetScale) {
            GetComponent<RectTransform>().localScale = targetScale;
        }
    }

    public void SetDragged(bool isBeingDragged) {
        beingDragged = isBeingDragged;
        CardManager.singleton.currentlyDraggedCard = isBeingDragged ? this : null;
    }

    public void SetTargetPosition(Vector3 target) {
        targetLocation = new Vector3(target.x, target.y - (GetComponent<RectTransform>().rect.height/4), target.z);
    }

    public void SetScale(Vector3 scale) {
        targetScale = scale;
        GetComponent<RectTransform>().localScale = targetScale;
    }

    public bool CanHoverCard() {
        return CardManager.singleton.currentlyDraggedCard == null || CardManager.singleton.currentlyDraggedCard == this;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        // TODO make sure the player isnt dragging another card
        if (beingDragged || !CanHoverCard()) {
            return;
        }

        originalParent = transform.parent;

        CreatePlaceholder();

        // Remove from the hand
        transform.SetParent(GameObject.Find("GameCanvas").transform);
        targetScale = new Vector3(1, 1, 1);
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (beingDragged || placeholder == null || !CanHoverCard()) {
            return;
        }

        targetScale = new Vector3(0.66f, 0.66f, 1);
        ReturnToParent(eventData.position);
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (!GetComponent<CardDisplay>().CanInterractWithCard()) {
            return;
        }

        SetDragged(true);

        SetTargetPosition(eventData.position);

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) {
        if (!GetComponent<CardDisplay>().CanInterractWithCard(false) || !beingDragged) {
            return;
        }

        SetTargetPosition(eventData.position);

        AdjustPlaceHolder();
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
            if (transform.position.x < placeholderParent.GetChild(i).position.x) {
                newSiblingIndex = i;

                if (placeholder.transform.GetSiblingIndex() < newSiblingIndex) {
                    newSiblingIndex--;
                }
                
                break;
            }
        }

        placeholder.transform.SetSiblingIndex(newSiblingIndex);
    }

    public void ReturnToParent(Vector2 position) {
        SetTargetPosition(position);

        // Jump back into the hand
        transform.SetParent(originalParent);

        // Place card at correct index in hand
        transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());

        targetScale = new Vector3(0.66f, 0.66f, 1);

        Destroy(placeholder);
    }
}
