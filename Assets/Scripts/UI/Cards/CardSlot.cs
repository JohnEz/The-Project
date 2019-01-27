using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum CardSlotState {
    NONE,
    DISPLAYING_DETAILS,
    DRAGGING,
    PLAYED
}

public class CardSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    public GameObject hoverCardDisplayPrefab;
    public CardDisplay cardSlotDisplay;

    private CardDisplay hoverCardDisplay;

    public AbilityCardBase card;
    public CardSlotState state;

    public PlayerUnit myUnit;
    public Hand myHand;

    public void Start() {
        cardSlotDisplay.SetCardAbility(card);
        state = CardSlotState.NONE;

        if (myUnit != null) {
            card.caster = myUnit.unit;
        } else {
            Debug.Log("Card was started without unit set!");
        }
    }

    public void Update() {
        // TODO this seems in efficient
        bool glowOn = UserInterfaceManager.singleton.CanPlayCard(this);

        cardSlotDisplay.SetGlow(glowOn);

        if (hoverCardDisplay != null) {
            hoverCardDisplay.SetGlow(glowOn);
        }
    }

    public void OnDestroy() {
        if (myHand != null) {
            myHand.CardDestroyed(gameObject);
        }
    }

    private void CreateHoverCardDisplay() {
        if (hoverCardDisplay != null) {
            Debug.LogError("Tried to create hover card when it already exists");
        }

        GameObject hoverCardDisplayObject = Instantiate(hoverCardDisplayPrefab, transform);
        hoverCardDisplayObject.transform.SetParent(GetComponentInParent<Canvas>().transform);
        hoverCardDisplay = hoverCardDisplayObject.GetComponent<CardDisplay>();
        hoverCardDisplay.SetCardAbility(card);
    }

    private void DestroyHoverCardDisplay(float delay = 0) {
        if (hoverCardDisplay != null) {
            Destroy(hoverCardDisplay.gameObject, delay);
        }
    }

    public void DisplayDetailedCard() {
        CreateHoverCardDisplay();
        cardSlotDisplay.gameObject.SetActive(false);
    }

    public void HideDetailedCard() {
        DestroyHoverCardDisplay();
        cardSlotDisplay.gameObject.SetActive(true);
    }

    public void PlayCard() {
        state = CardSlotState.PLAYED;
        gameObject.SetActive(false);
        HideDetailedCard();
    }

    public void CancelCard() {
        state = CardSlotState.NONE;
        gameObject.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (state == CardSlotState.NONE) {
            UserInterfaceManager.singleton.CardHovered(this);
            state = CardSlotState.DISPLAYING_DETAILS;
            DisplayDetailedCard();
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (state == CardSlotState.DISPLAYING_DETAILS) {
            UserInterfaceManager.singleton.CardUnhovered();
            state = CardSlotState.NONE;
            HideDetailedCard();
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        //    state = CardSlotState.DRAGGING;
        //    HideDetailedCard();
    }

    public void OnDrag(PointerEventData eventData) {
        //    cardSlotDisplay.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData) {
        //    state = CardSlotState.NONE;
        //    cardSlotDisplay.transform.localPosition = Vector3.zero;
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (UserInterfaceManager.singleton.PlayCard(this)) {
            PlayCard();
        }
    }
}