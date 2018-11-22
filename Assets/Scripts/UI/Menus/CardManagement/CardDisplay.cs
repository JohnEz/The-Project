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
        UserInterfaceManager uiManager = GameObject.Find("Game Controller").GetComponent<UserInterfaceManager>();
        Draggable dragCompoment = GetComponent<Draggable>();

        // if it was not dropped back into the hand
        if (!dragCompoment.droppedOnZone && uiManager.CanPlayCard()) {
            CardPlayed();
        }
    }

    private void CardPlayed() {
        UserInterfaceManager uiManager = GameObject.Find("Game Controller").GetComponent<UserInterfaceManager>();
        uiManager.CardPlayed(ability);

        Destroy(this.gameObject);
    }


    public void OnPointerEnter(PointerEventData eventData) {
        UserInterfaceManager uiManager = GameObject.Find("Game Controller").GetComponent<UserInterfaceManager>();
        uiManager.CardHovered(ability);
    }

    public void OnPointerExit(PointerEventData eventData) {
        UserInterfaceManager uiManager = GameObject.Find("Game Controller").GetComponent<UserInterfaceManager>();
        uiManager.CardUnhovered();
    }

    void Start()
    {
        nameText.text = ability.name;
        artworkImage.sprite = ability.icon;
        descriptionText.text = ability.GetDescription();
    }



}
