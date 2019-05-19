using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SchoolList : MonoBehaviour {
    public GameObject characterCardPrefab;

    public Dropzone myDropzone;

    public Transform listTransform;

    public void Start() {
        GenerateCharacterList();
    }

    private void GenerateCharacterList() {
        PlayerSchool.Roster.ForEach((unit) => {
            CreateCharacterCard(unit);
        });
    }

    private void CreateCharacterCard(UnitObject character) {
        GameObject characterCard = Instantiate(characterCardPrefab, myDropzone.ZoneTransform);

        characterCard.GetComponent<Draggable>().SetNewParent(myDropzone);
        characterCard.GetComponent<CharacterCard>().MyCharacter = character;
    }
}