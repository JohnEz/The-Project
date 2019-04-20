using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SchoolList : MonoBehaviour {

    //TEMP - roster should be set up by new game menu or something
    public List<UnitObject> defaultCharacters;

    public GameObject characterCardPrefab;

    public Dropzone myDropzone;

    public Transform listTransform;

    public void Start() {
        //TEMP
        if (PlayerSchool.Roster.Count <= 0) {
            defaultCharacters.ForEach((character) => {
                PlayerSchool.Roster.Add(Instantiate(character));
            });
        }

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