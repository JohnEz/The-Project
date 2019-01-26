﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandController : MonoBehaviour {
    //public class HandController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    //private const float DRAG_SPEED = 25F;
    //private const float MIN_DISTANCE = 1f;

    //private Vector3 targetLocation;

    //public Transform originalParent;
    //public Transform placeholderParent;

    //private GameObject placeholder = null;

    //private bool beingDragged = false;
    //public bool droppedOnZone = false;

    //private void Start() {
    //}

    //private void Update() {
    //    if (beingDragged && Vector3.Distance(targetLocation, transform.position) > MIN_DISTANCE) {
    //        this.transform.position = Vector3.Lerp(transform.position, targetLocation, DRAG_SPEED * Time.deltaTime);
    //    }
    //}

    //public void OnBeginDrag(PointerEventData eventData) {
    //    if (!GetComponent<CardDisplay>().CanInterractWithCard()) {
    //        return;
    //    }

    //    beingDragged = true;
    //    originalParent = transform.parent;

    //    CreatePlaceholder();

    //    // Remove from the hand
    //    transform.SetParent(GameObject.Find("GameCanvas").transform);

    //    targetLocation = eventData.position;

    //    GetComponent<CanvasGroup>().blocksRaycasts = false;
    //}

    //public void OnDrag(PointerEventData eventData) {
    //    if (!GetComponent<CardDisplay>().CanInterractWithCard(false)) {
    //        return;
    //    }

    //    targetLocation = eventData.position;

    //    AdjustPlaceHolder();
    //}

    //public void OnDestroy() {
    //    Destroy(placeholder);
    //}

    //public void OnEndDrag(PointerEventData eventData) {
    //    if (!beingDragged) {
    //        return;
    //    }

    //    beingDragged = false;
    //    ReturnToParent(eventData.position);

    //    droppedOnZone = false;

    //    GetComponent<CanvasGroup>().blocksRaycasts = true;
    //}

    //// Creates a placeholder where this card was in the hand
    //private void CreatePlaceholder() {
    //    RectTransform myRect = GetComponent<RectTransform>();

    //    placeholderParent = originalParent;

    //    placeholder = new GameObject();
    //    placeholder.transform.SetParent(placeholderParent);
    //    RectTransform rect = placeholder.AddComponent<RectTransform>();
    //    rect.sizeDelta = myRect.sizeDelta;

    //    LayoutElement le = placeholder.AddComponent<LayoutElement>();
    //    le.preferredWidth = GetComponent<LayoutElement>().preferredWidth;
    //    le.preferredHeight = GetComponent<LayoutElement>().preferredHeight;
    //    le.flexibleHeight = 0;
    //    le.flexibleWidth = 0;

    //    placeholder.transform.SetSiblingIndex(transform.GetSiblingIndex());
    //}

    //private void AdjustPlaceHolder() {
    //    int newSiblingIndex = placeholderParent.childCount;

    //    if (placeholder.transform.parent != placeholderParent) {
    //        placeholder.transform.SetParent(placeholderParent);
    //    }

    //    for (int i = 0; i < placeholderParent.childCount; i++) {
    //        if (transform.position.x < placeholderParent.GetChild(i).position.x) {
    //            newSiblingIndex = i;

    //            if (placeholder.transform.GetSiblingIndex() < newSiblingIndex) {
    //                newSiblingIndex--;
    //            }

    //            break;
    //        }
    //    }

    //    placeholder.transform.SetSiblingIndex(newSiblingIndex);
    //}

    //public void ReturnToParent(Vector2 position) {
    //    targetLocation = position;

    //    // Jump back into the hand
    //    transform.SetParent(originalParent);

    //    // Place card at correct index in hand
    //    transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());

    //    Destroy(placeholder);
    //}
}