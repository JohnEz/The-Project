using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDescriptionController : MonoBehaviour {
    public GameObject abilityDescriptionPrefab;

    public TextMeshProUGUI exhaustText;

    public List<GameObject> abilityDescriptions;

    public void SetDescription(AbilityCardBase ability) {
        ClearDescription();
        ability.Actions.ForEach(action => {
            GameObject createdAction = Instantiate(abilityDescriptionPrefab, transform);
            createdAction.GetComponent<ActionDescriptionController>().SetAction(action);
        });

        exhaustText.gameObject.SetActive(ability.exhausts);
        exhaustText.text = "Exhaust";
    }

    public void ClearDescription() {
        abilityDescriptions.ForEach(description => {
            Destroy(description);
        });
        abilityDescriptions.Clear();
    }
}