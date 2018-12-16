using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDescriptionController : MonoBehaviour {

    public GameObject abilityDescriptionPrefab;

    public List<GameObject> abilityDescriptions;

    public void SetDescription(AbilityCardBase ability) {
        ClearDescription();
        ability.Actions.ForEach(action => {
            GameObject createdAction = Instantiate(abilityDescriptionPrefab, transform);
            createdAction.GetComponent<ActionDescriptionController>().SetAction(action);
        });
    }

    public void ClearDescription() {
        abilityDescriptions.ForEach(description => {
            Destroy(description);
        });
        abilityDescriptions.Clear();
    }
}
