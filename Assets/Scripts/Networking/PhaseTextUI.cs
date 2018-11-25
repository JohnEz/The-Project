using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhaseTextUI : MonoBehaviour {

    public string[] PHASE_TEXT = {
        "Turn Starting",
        "Waiting for Input",
        "Unit Moving",
        "Unit Attack",
        "Turn Ending",
        "Waiting for Players"
    };

    TurnManager turnManager;
    Text text;

    // Use this for initialization
    void Start () {
        turnManager = GameObject.FindObjectOfType<TurnManager>();
        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        if (turnManager == null) {
            turnManager = GameObject.FindObjectOfType<TurnManager>();
        } else {
            text.text = PHASE_TEXT[(int)turnManager.CurrentPhase];
        }

        
	}
}
