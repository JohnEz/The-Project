using DuloGames.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Ability", menuName = "Ability/New Ability")]
public class Ability : ScriptableObject {

    [Serializable] public class OnCooldownChangeEvent : UnityEvent<UIAbilityInfo> { }

    public OnCooldownChangeEvent onCooldownChange = new OnCooldownChangeEvent();

    [Serializable] public class OnUsesChangeEvent : UnityEvent<UIAbilityInfo> { }

    public OnUsesChangeEvent onUsesChange = new OnUsesChangeEvent();

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

    public int baseCooldown = 1;
    public int baseUses = -1;

    [HideInInspector]
    private int cooldown;

    [HideInInspector]
    public int maxCooldown = 1;

    [HideInInspector]
    private int remainingUses;

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
        remainingUses = baseUses;
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
        string updatedDescription = description;

        int damage = 0;
        int healing = 0;
        int shield = 0;

        instansiatedActions.ForEach((action) => {
            damage += action.GetDamage(caster);
            healing += action.GetHealing(caster);
            shield += action.GetShield(caster);
        });

        updatedDescription = updatedDescription.Replace("{damage}", damage + " damage");
        updatedDescription = updatedDescription.Replace("{healing}", healing + " healing");
        updatedDescription = updatedDescription.Replace("{shield}", shield + " shield");

        return updatedDescription;
    }

    public int Cooldown {
        get { return cooldown; }
        set {
            cooldown = value;

            if (this.onCooldownChange != null)
                this.onCooldownChange.Invoke(ToAbilityInfo());
        }
    }

    public int MaxCooldown {
        get { return maxCooldown; }
        set { maxCooldown = value; }
    }

    public bool IsOnCooldown() {
        return Cooldown > 0;
    }

    public int RemainingUses {
        get { return remainingUses; }
        set {
            remainingUses = value;

            if (this.onUsesChange != null)
                this.onUsesChange.Invoke(ToAbilityInfo());
        }
    }

    public bool HasRemainingUses() {
        return baseUses == -1 || remainingUses > 0;
    }

    public void SetOnCooldown(bool isOnCooldown) {
        Cooldown = isOnCooldown ? maxCooldown : 0;
    }

    private int GetRange() {
        //TEMP
        AbilityAction firstAttackAction = instansiatedActions.Find(action => action.GetType() == typeof(AttackAction));

        if (firstAttackAction != null) {
            AttackAction attackAction = (AttackAction)firstAttackAction;
            switch (attackAction.areaOfEffect) {
                case AreaOfEffect.AURA: return attackAction.aoeRange;
                default: return attackAction.range;
            }
        }

        return 1;
    }

    public UIAbilityInfo ToAbilityInfo() {
        UIAbilityInfo info = new UIAbilityInfo();

        info.ID = GetInstanceID();
        info.Name = Name;
        info.Icon = icon;
        info.Description = caster != null ? GetDescription() : "";
        info.Cooldown = cooldown;
        info.MaxCooldown = MaxCooldown;
        info.CastTime = 0;
        info.PowerCost = baseActionPointCost;
        info.ability = this;
        info.Range = GetRange();
        info.RemainingUses = remainingUses;

        return info;
    }
}