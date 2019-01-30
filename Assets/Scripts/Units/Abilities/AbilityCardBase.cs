using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "Card/New Card")]
public class AbilityCardBase : ScriptableObject {
    public new string name = "UNNAMED";
    public string description = "No description set!";
    public Sprite icon;

    public List<CardAction> baseActions = new List<CardAction>(0);

    [HideInInspector]
    public List<CardAction> instansiatedActions;

    public int staminaCost = 1;

    public UnitController caster;

    public bool exhausts = false;

    public void Awake() {
        instansiatedActions = new List<CardAction>(0);
        baseActions.ForEach(action => {
            instansiatedActions.Add(Instantiate(action));
        });
    }

    public List<CardAction> Actions {
        get { return instansiatedActions; }
    }

    public string Name {
        get { return name; }
        set { name = value; }
    }

    public virtual string GetDescription() {
        return description;
    }
}