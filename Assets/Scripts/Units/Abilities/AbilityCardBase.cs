using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "Ability/New Ability")]
public class AbilityBase : ScriptableObject {
    public new string name = "UNNAMED";
    public string description = "No description set!";
    public Sprite icon;

    public List<AbilityAction> baseActions = new List<AbilityAction>(0);

    [HideInInspector]
    public List<AbilityAction> instansiatedActions;

    public int staminaCost = 1;

    [HideInInspector]
    public UnitController caster;

    public void Awake() {
        instansiatedActions = new List<AbilityAction>(0);
        baseActions.ForEach(action => {
            instansiatedActions.Add(Instantiate(action));
        });
    }

    public List<AbilityAction> Actions {
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