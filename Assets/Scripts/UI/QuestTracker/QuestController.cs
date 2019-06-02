using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class QuestController : MonoBehaviour {
    public TextMeshProUGUI title;
    public TextMeshProUGUI objectiveText;

    public GameObject completeGraphic;
    public GameObject failedGraphic;

    public void Initiate(Objective objective) {
        title.text = objective.title;
        objectiveText.text = objective.text;
        objective.onObjectiveUpdated.AddListener(OnObjectiveUpdate);
    }

    public void OnObjectiveUpdate(ObjectiveStatus newStatus) {
        switch (newStatus) {
            case ObjectiveStatus.NONE:
                completeGraphic.SetActive(false);
                failedGraphic.SetActive(false);
                break;

            case ObjectiveStatus.COMPLETE:
                completeGraphic.SetActive(true);
                failedGraphic.SetActive(false);
                break;

            case ObjectiveStatus.FAILED:
                completeGraphic.SetActive(false);
                failedGraphic.SetActive(true);
                break;
        }
    }
}