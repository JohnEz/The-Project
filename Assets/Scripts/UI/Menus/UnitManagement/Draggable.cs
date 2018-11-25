﻿using UnityEngine;
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
    public bool droppedOnZone = false;

    void Start() {
        
    }

    void Update() {
        if (beingDragged && Vector3.Distance(targetLocation, transform.position) > MIN_DISTANCE) {
            this.transform.position = Vector3.Lerp(transform.position, targetLocation, DRAG_SPEED * Time.deltaTime);
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (!CanInterractWithCard()) {
            return;
        }

        beingDragged = true;
        originalParent = transform.parent;
        
        CreatePlaceholder();
        
        // Remove from the hand
        transform.SetParent(GameObject.Find("GameCanvas").transform);

        targetLocation = eventData.position;

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) {
        if (!CanInterractWithCard()) {
            return;
        }

        targetLocation = eventData.position;

        AdjustPlaceHolder();
    }

    public void OnDestroy() {
        Destroy(placeholder);
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (!CanInterractWithCard()) {
            return;
        }

        beingDragged = false;
        if (droppedOnZone) {
            Debug.Log("i was dropped on a zone!");
            targetLocation = eventData.position;

            // Jump back into the hand
            transform.SetParent(originalParent);

            // Place card at correct index in hand
            transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());

            droppedOnZone = false;
        }

        Destroy(placeholder);

        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    private bool CanInterractWithCard() {
        CardSlot myCardSlot = GetComponent<CardSlot>();
        if (!myCardSlot.myPlayer.hasAuthority) {
            Debug.LogError("This card does not belong to you!");
            return false;
        }

        if (!myCardSlot.myPlayer.isMyTurn) {
            Debug.LogError("Its not your turn!");
            return false;
        }
        return true;
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
}
