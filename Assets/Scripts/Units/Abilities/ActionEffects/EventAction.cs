using UnityEngine;
using System.Collections;

public class EventAction : ScriptableObject {

    public AbilityEvent eventTrigger;
    public EventTarget eventTarget;
    public System.Action<UnitController, UnitController, Node> action;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
