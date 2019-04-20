using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dropzone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
    public Transform zoneTransform;

    private Dictionary<string, System.Func<GameObject, bool>> dropFilters;
    private Dictionary<string, System.Action<GameObject>> dropListeners;
    private Dictionary<string, System.Action<GameObject>> removeListeners;

    private void Awake() {
        dropFilters = new Dictionary<string, System.Func<GameObject, bool>>();
        dropListeners = new Dictionary<string, System.Action<GameObject>>();
        removeListeners = new Dictionary<string, System.Action<GameObject>>();
    }

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

        bool failedFilter = false;
        foreach (System.Func<GameObject, bool> filter in dropFilters.Values) {
            if (!filter(eventData.pointerDrag)) {
                failedFilter = true;
            }
        }

        if (failedFilter) {
            return;
        }

        draggedItem.SetNewParent(this);

        foreach (System.Action<GameObject> listener in dropListeners.Values) {
            listener(eventData.pointerDrag);
        }
    }

    public void RemoveDraggedItem(GameObject go) {
        foreach (System.Action<GameObject> listener in removeListeners.Values) {
            listener(go);
        }
    }

    public void AddDropFilter(string id, System.Func<GameObject, bool> filter) {
        dropFilters.Add(id, filter);
    }

    public void RemoveDropFilter(string id) {
        dropFilters.Remove(id);
    }

    public void AddDropListener(string id, System.Action<GameObject> listener) {
        dropListeners.Add(id, listener);
    }

    public void RemoveDropListener(string id) {
        dropListeners.Remove(id);
    }

    public void AddRemoveListener(string id, System.Action<GameObject> listener) {
        removeListeners.Add(id, listener);
    }

    public void RemoveRemoveListener(string id) {
        removeListeners.Remove(id);
    }
}