using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PartyList : MonoBehaviour {
    private Dropzone myDropzone;

    public void Start() {
        myDropzone = GetComponentInChildren<Dropzone>();

        myDropzone.AddDropFilter("PartySize", PartySizeLimitFilter);
        myDropzone.AddDropListener("Add Character", AddCharacter);
        myDropzone.AddRemoveListener("Remove Character", RemoveCharacter);
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
    }
}