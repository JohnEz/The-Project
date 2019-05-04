using UnityEngine;
using TMPro;

public class AbilityDescriptionController : MonoBehaviour {
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI costText;

    public void ShowDescription(Ability ability) {
        gameObject.SetActive(true);
        SetDescription(ability);
    }

    public void HideDescription() {
        gameObject.SetActive(false);
    }

    public void SetDescription(Ability ability) {
        titleText.text = ability.Name;
        descriptionText.text = ability.description;
        costText.text = ability.actionPointCost + " AP";
    }
}