using System.Collections.Generic;
using UnityEngine;


public enum CardState {
    NONE,
    PLAYED,
    INVOKED
}

public class CardManager : MonoBehaviour {
    public const int DECK_SIZE = 30;

    public static CardManager singleton;

    public Draggable currentlyDraggedCard; // TODO this should be moved

    private CardSlot activeCard = null;
    private int currentActionIndex = 0;
    private CardState cardState = CardState.NONE;

    public CardSlot ActiveCard {
        get { return activeCard; }
        set { activeCard = value; }
    }

    public int CurrentActionIndex {
        get { return currentActionIndex; }
        set { currentActionIndex = value; }
    }

    private void Awake() {
        singleton = this;
    }

    public List<AbilityCardBase> LoadDeck(List<string> deckList) {
        List<AbilityCardBase> loadedDeck = new List<AbilityCardBase>();

        deckList.ForEach(cardName => {
            AbilityCardBase cardToAdd = ResourceManager.singleton.cards[cardName];

            if (cardToAdd != null) {
                loadedDeck.Add(cardToAdd);
            }
        });

        return loadedDeck;
    }

    // Playing Cards
    /////////////////
    public bool IsACardPlayed() {
        return cardState == CardState.PLAYED;
    }

    public bool IsACardInvoked() {
        return cardState == CardState.INVOKED;
    }

    public bool IsACardActive() {
        return cardState != CardState.NONE;
    }

    // When the user clicked an action from the card
    public void CardInvoked() {
        cardState = CardState.INVOKED;
    }

    public bool CanPlayCard() {
        return
            !CardManager.singleton.IsACardActive() &&
            TurnManager.singleton.CurrentPhase == TurnPhase.WAITING_FOR_INPUT &&
            TurnManager.singleton.IsPlayersTurn();
    }

    public bool CanPlayCard(CardSlot cardSlot) {
        return CanPlayCard() && cardSlot.card.caster.Stamina >= cardSlot.card.staminaCost;
    }

    public void PlayCard(CardSlot cardSlot) {
        cardState = CardState.PLAYED;
        cardSlot.card.caster.Stamina -= cardSlot.card.staminaCost;
        ActiveCard = cardSlot;
        UserInterfaceManager.singleton.RunNextCardAction(ActiveCard, CurrentActionIndex);
    }

    public void CancelCurrentCard() {
        if (IsACardPlayed()) {
            cardState = CardState.NONE;
            ActiveCard.CancelCard();
            ActiveCard.card.caster.Stamina += ActiveCard.card.staminaCost;
            UserInterfaceManager.singleton.UnshowCard();
        }
    }

    public void DestroyActiveCard() {
        Debug.Log(ActiveCard);
        Debug.Log(ActiveCard.myUnit);
        Debug.Log(ActiveCard.myUnit.discard);
        Debug.Log(ActiveCard.card);
        ActiveCard.myUnit.discard.Push(ActiveCard.card);
        Destroy(ActiveCard.gameObject);
        ActiveCard = null;
        cardState = CardState.NONE;
        CurrentActionIndex = 0;
    }
}