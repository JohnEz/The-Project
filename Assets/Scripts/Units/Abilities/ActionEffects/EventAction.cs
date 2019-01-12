using UnityEngine;

public class EventAction : ScriptableObject {
    public AbilityEvent eventTrigger;
    public EventTarget eventTarget;
    public System.Action<UnitController, Node> action;

    // Use this for initialization
    private void Start() {
    }

    // Update is called once per frame
    private void Update() {
    }
}