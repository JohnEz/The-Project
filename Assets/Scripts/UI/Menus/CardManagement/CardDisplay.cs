using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardDisplay : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {

    public AbilityCardBase ability;

    public Text nameText;
    public Image artworkImage;
    public Text descriptionText;

    public Player myPlayer;

    void Start() {
        nameText.text = ability.name;
        artworkImage.sprite = ability.icon;
        descriptionText.text = ability.GetDescription();
        ability.caster = myPlayer.myCharacter;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        //throw new System.NotImplementedException();
    }

    public void OnDrag(PointerEventData eventData) {
        //throw new System.NotImplementedException();
    }

    public void OnDrop(PointerEventData eventData) {
        //throw new System.NotImplementedException();
    }

    public void OnEndDrag(PointerEventData eventData) {
        Draggable dragCompoment = GetComponent<Draggable>();

        // if it was not dropped back into the hand
        if (!dragCompoment.droppedOnZone && UserInterfaceManager.singleton.CanPlayCard()) {
            CardPlayed();
        }
    }

    private void CardPlayed() {
        UserInterfaceManager.singleton.CardPlayed(this);
        gameObject.SetActive(false);
    }


    public void OnPointerEnter(PointerEventData eventData) {
        UserInterfaceManager.singleton.CardHovered(this);
    }

    public void OnPointerExit(PointerEventData eventData) {
        UserInterfaceManager.singleton.CardUnhovered();
    }
}
