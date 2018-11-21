using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Dropzone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

    public void OnPointerEnter(PointerEventData eventData) {
        if (eventData.pointerDrag == null) {
            return;
        }

        Draggable draggedItem = eventData.pointerDrag.GetComponent<Draggable>();

        if (draggedItem != null) {
            draggedItem.placeholderParent = transform;
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (eventData.pointerDrag == null) {
            return;
        }

        Draggable draggedItem = eventData.pointerDrag.GetComponent<Draggable>();

        if (draggedItem != null && draggedItem.originalParent == transform) {
            draggedItem.placeholderParent = draggedItem.originalParent;
        }
    }

    public void OnDrop(PointerEventData eventData) {
        Draggable draggedItem = eventData.pointerDrag.GetComponent<Draggable>();

        if (draggedItem != null) {
            draggedItem.originalParent = transform;
            draggedItem.droppedOnZone = true;
        }
    }
}
