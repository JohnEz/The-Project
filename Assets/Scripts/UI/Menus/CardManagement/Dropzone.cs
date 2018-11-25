using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class Dropzone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField]
    public int testInt;

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
            Debug.Log("Dropped on zone!");
            draggedItem.droppedOnZone = true;
        }
    }

    void Update() {
        //Debug.Log("Hand " + testInt + "'s id is " + GetComponent<NetworkIdentity>().netId);
    }
}
