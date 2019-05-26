using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PartyList : MonoBehaviour {

    [Serializable] public class OnAddEvent : UnityEvent<int> { }

    [Serializable] public class OnRemoveEvent : UnityEvent<int> { }

    public OnAddEvent onAdd = new OnAddEvent();
    public OnRemoveEvent onRemove = new OnRemoveEvent();

    public TextMeshProUGUI partyLimitText;
    public Dropzone myDropzone;

    public void Start() {
        myDropzone = GetComponentInChildren<Dropzone>();

        myDropzone.AddDropFilter("PartySize", PartySizeLimitFilter);
        myDropzone.AddDropListener("Add Character", AddCharacter);
        myDropzone.AddRemoveListener("Remove Character", RemoveCharacter);

        // TODO maybe remove this and keep the last used party?
        GameDetails.Party.Clear();
        UpdatePartyLimitText();
    }

    public bool PartySizeLimitFilter(GameObject go) {
        return GameDetails.Party.Count < GameDetails.MaxPartySize;
    }

    public void AddCharacter(GameObject go) {
        CharacterCard addedCard = go.GetComponent<CharacterCard>();

        if (!addedCard) {
            Debug.LogError("Dragged not character card into party list!");
            return;
        }

        UnitObject addedUnit = addedCard.MyCharacter;

        if (!addedUnit) {
            Debug.LogError("Character card didn't have a unit?!");
            return;
        }

        GameDetails.Party.Add(addedUnit);
        UpdatePartyLimitText();

        if (onAdd != null) {
            onAdd.Invoke(GameDetails.Party.Count);
        }
    }

    public void RemoveCharacter(GameObject go) {
        CharacterCard removedCard = go.GetComponent<CharacterCard>();
        if (!removedCard) {
            Debug.LogError("Tried to remove non character card?!");
            return;
        }

        UnitObject removedUnit = removedCard.MyCharacter;

        if (!removedUnit) {
            Debug.LogError("Character card didn't have a unit?!");
            return;
        }

        GameDetails.Party.Remove(removedUnit);
        UpdatePartyLimitText();

        if (onRemove != null) {
            onRemove.Invoke(GameDetails.Party.Count);
        }
    }

    public void UpdatePartyLimitText() {
        if (partyLimitText == null) {
            return;
        }

        partyLimitText.text = GameDetails.Party.Count + "/" + GameDetails.MaxPartySize;
    }
}