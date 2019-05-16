using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QuestController : MonoBehaviour {
    public Text title;
    public Text objective;

    // Use this for initialization
    private void Start() {
    }

    // Update is called once per frame
    private void Update() {
    }

    public void Initiate(string titleString, string objectiveString) {
        title.text = titleString;
        objective.text = objectiveString;
    }
}