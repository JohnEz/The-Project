using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "Ability/New Ability")]
public class Ability : ScriptableObject {
    public new string name = "UNNAMED";
    public string description = "No description set!";
    public Sprite icon;

    public List<AbilityAction> baseActions = new List<AbilityAction>(0);

    [HideInInspector]
    public List<AbilityAction> instansiatedActions;

    public int baseActionPointCost = 1;

    [HideInInspector]
    public int actionPointCost;

    public int staminaCost = 1;

    [HideInInspector]
    public UnitController caster;

    [HideInInspector]
    public int cooldown;

    public int baseCooldown = 1;

    [HideInInspector]
    public int maxCooldown = 1;

    public void Awake() {
        instansiatedActions = new List<AbilityAction>(0);
        baseActions.ForEach(action => {
            if (!action) {
                Debug.LogError("Instantiated ability incorrectly: " + name);
                return;
            }
            instansiatedActions.Add(Instantiate(action));
        });
        maxCooldown = baseCooldown;
        cooldown = 0;
        actionPointCost = baseActionPointCost;
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

    public int Cooldown {
        get { return cooldown; }
        set { cooldown = value; }
    }

    public int MaxCooldown {
        get { return maxCooldown; }
        set { maxCooldown = value; }
    }

    public bool IsOnCooldown() {
        return Cooldown > 0;
    }

    public void SetOnCooldown(bool isOnCooldown) {
        cooldown = isOnCooldown ? maxCooldown : 0;
    }
}