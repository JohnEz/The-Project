using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Dropzone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
    public Transform zoneTransform;

    [Serializable] public class OnDropEvent : UnityEvent<GameObject> { }

    [Serializable] public class OnRemoveEvent : UnityEvent<GameObject> { }

    public OnDropEvent onDrop = new OnDropEvent();
    public OnRemoveEvent onRemove = new OnRemoveEvent();

    private Dictionary<string, System.Func<GameObject, bool>> dropFilters;

    public Transform ZoneTransform {
        get { return zoneTransform ? zoneTransform : transform; }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (eventData.pointerDrag == null) {
            return;
        }

        Draggable draggedItem = eventData.pointerDrag.GetComponent<Draggable>();

        if (draggedItem != null) {
            draggedItem.placeholderParent = ZoneTransform;
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (eventData.pointerDrag == null) {
            return;
        }

        Draggable draggedItem = eventData.pointerDrag.GetComponent<Draggable>();

        if (draggedItem != null && draggedItem.originalParent == ZoneTransform) {
            draggedItem.placeholderParent = draggedItem.originalParent;
        }
    }

    public void OnDrop(PointerEventData eventData) {
        Draggable draggedItem = eventData.pointerDrag.GetComponent<Draggable>();

        if (draggedItem == null) {
            return;
        }

        if (dropFilters != null) {
            bool failedFilter = false;
            foreach (System.Func<GameObject, bool> filter in dropFilters.Values) {
                if (!filter(eventData.pointerDrag)) {
                    failedFilter = true;
                }
            }

            if (failedFilter) {
                return;
            }
        }

        draggedItem.SetNewParent(this);

        if (onDrop != null) {
            onDrop.Invoke(eventData.pointerDrag);
        }
    }

    public void RemoveDraggedItem(GameObject go) {
        if (onRemove != null) {
            onRemove.Invoke(go);
        }
    }

    public void AddDropFilter(string id, System.Func<GameObject, bool> filter) {
        if (dropFilters == null) {
            dropFilters = new Dictionary<string, System.Func<GameObject, bool>>();
        }
        dropFilters.Add(id, filter);
    }

    public void RemoveDropFilter(string id) {
        if (dropFilters == null) {
            return;
        }
        dropFilters.Remove(id);
    }
}