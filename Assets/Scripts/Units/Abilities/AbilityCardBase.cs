using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Ability", menuName = "Card/BaseCard")]
public class AbilityCardBase : ScriptableObject {

    public new string name = "UNNAMED";
    public string description = "No description set!";
    public Sprite icon;

    public List<CardAction> myActions = new List<CardAction>(0);

    public int staminaCost = 3;

    public UnitController caster;

    public AbilityCardBase(List<EventAction> _eventActions, UnitController _caster)
    {
        caster = _caster;
    }

    public List<CardAction> Actions
    {
        get { return myActions; }
    }

    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public virtual string GetDescription()
    {
        return description;
    }

}
