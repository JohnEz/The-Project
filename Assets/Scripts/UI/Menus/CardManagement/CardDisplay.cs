using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public AbilityCardBase ability;

    public TextMeshProUGUI nameText;
    public Image artworkImage;
    public Text descriptionText;
    public CardDescriptionController descriptionController;
    public TextMeshProUGUI staminaText;

    public PlayerUnit myUnit;
    public Hand myHand;

    private void Start() {
        nameText.text = ability.name;
        artworkImage.sprite = ability.icon;
        //descriptionText.text = ability.GetDescription();
        descriptionController.SetDescription(ability);
        staminaText.text = ability.staminaCost.ToString();

        if (myUnit != null) {
            ability.caster = myUnit.unit;
        } else {
            Debug.LogError("Card was started without unit set!");
        }
    }

    public bool CanInterractWithCard(bool displayErrors = true) {
        // check players turn
        if (TurnManager.singleton.GetCurrentPlayer() != myUnit.unit.myPlayer) {
            if (displayErrors) {
                GUIController.singleton.ShowErrorMessage("You already have a card played");
            }
            return false;
        }

        // check can play card
        if (!UserInterfaceManager.singleton.CanPlayCard()) {
            if (displayErrors) {
                GUIController.singleton.ShowErrorMessage("You already have a card played");
            }
            return false;
        }

        // if i have the stamina for it
        AbilityCardBase ability = GetComponent<CardDisplay>().ability;
        if (ability.staminaCost > ability.caster.Stamina) {
            if (displayErrors) {
                GUIController.singleton.ShowErrorMessage("Not enough Stamina");
            }
            return false;
        }

        return true;
    }

    public void OnDestroy() {
        myHand.CardDestroyed(gameObject);
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
        if (!dragCompoment.droppedOnZone && CanInterractWithCard(false)) {
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