using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class CardSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {

    public bool isActive = false;
    public int slotIndex;

    public CardId cardId;

    AbilityCardBase myAbilityCard;

    public Text nameText;
    public Image artworkImage;
    public Text descriptionText;

    public PlayerConnectionObject myPlayer;

    public void ShowCard(CardId CardId) {
        cardId = CardId;

        myAbilityCard = CardManager.singleton.cards[cardId.id];

        nameText.text = myAbilityCard.name;
        artworkImage.sprite = myAbilityCard.icon;
        descriptionText.text = myAbilityCard.GetDescription();
        SetActive(true);
    }

    public void ClearSlot() {
        SetActive(false);
        cardId = new CardId();
    }

    public void SetActive(bool active) {
        isActive = active;
        gameObject.SetActive(active);
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
        //UserInterfaceManager uiManager = GameObject.Find("Game Controller").GetComponent<UserInterfaceManager>();
        Draggable dragCompoment = GetComponent<Draggable>();

        // if it was not dropped back into the hand
        if (!dragCompoment.droppedOnZone && myPlayer.isMyTurn) {
            Debug.Log("i was played!");
            myPlayer.CmdPlayCard(cardId, slotIndex);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        //UserInterfaceManager uiManager = GameObject.Find("Game Controller").GetComponent<UserInterfaceManager>();
        //uiManager.CardHovered(ability);
    }

    public void OnPointerExit(PointerEventData eventData) {
        //UserInterfaceManager uiManager = GameObject.Find("Game Controller").GetComponent<UserInterfaceManager>();
        //uiManager.CardUnhovered();
    }

    void Start()
    {
        
    }

    void Update() {

    }



}
