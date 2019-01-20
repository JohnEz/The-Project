﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionDescriptionController : MonoBehaviour {
    public Sprite attackImage;
    public Sprite shieldImage;
    public Sprite moveImage;
    public Sprite drawImage;

    public Sprite burnImage;

    public Image actionImage;
    public TextMeshProUGUI actionText;

    //public Text quantityText;
    public Image buffImage;

    public GameObject subText;

    public void SetAction(CardAction action) {
        // Set the action icon and text
        if (typeof(AttackAction).IsAssignableFrom(action.GetType())) {
            AttackAction attackAction = (AttackAction)action;
            if (action.description != null && !action.description.Equals("")) {
                SetAttackText(attackAction);
            } else {
                SetAttackActionLegacy(attackAction);
            }
        } else if (action.GetType() == typeof(MoveAction)) {
            MoveAction moveAction = (MoveAction)action;
            SetMoveAction(moveAction);
        } else if (action.GetType() == typeof(DrawCardAction)) {
            DrawCardAction drawAction = (DrawCardAction)action;
            SetDrawAction(drawAction);
        }
    }

    private void SetAttackText(AttackAction attack) {
        int damage = 0;
        int healing = 0;
        int armour = 0;
        string stackString = null;
        string multiplyerString = null;

        attack.attackEffects.ForEach(effect => {
            switch(effect.GetType().ToString()) {
                case "DamageEffect":
                    DamageEffect damageEffect = (DamageEffect)effect;
                    damage += damageEffect.damage;
                    break;
                case "IncreaseArmour":
                    IncreaseArmour armourEffect = (IncreaseArmour)effect;
                    armour += armourEffect.armourIncrease;
                    break;
                case "DamagePerStackEffect":
                    DamagePerStackEffect damagePerStackEffect = (DamagePerStackEffect)effect;
                    stackString = damagePerStackEffect.ToDescription();
                    break;
                case "DamageWithMultiplierEffect":
                    DamageWithMultiplierEffect damageWithMultiplierEffect = (DamageWithMultiplierEffect)effect;
                    multiplyerString = damageWithMultiplierEffect.ToDescription();
                    break;
                case "HealEffect":
                    HealEffect healEffect = (HealEffect)effect;
                    healing += healEffect.healing;
                    break;
            }
        });

        bool displaySubText = false;
        string subTextString = "";

        if (attack.range > 1) {
            subTextString += "Range " + attack.range + "\n";
            displaySubText = true;
        }

        if (attack.areaOfEffect == AreaOfEffect.CIRCLE || attack.areaOfEffect == AreaOfEffect.AURA) {
            subTextString += "Area " + attack.aoeRange + "\n";
            displaySubText = true;
        } else if (attack.areaOfEffect == AreaOfEffect.CLEAVE) {
            subTextString += "Cleave\n";
            displaySubText = true;
        }

        actionText.text = string.Format(attack.description, damage, healing, armour, stackString, multiplyerString);

        if (displaySubText) {
            subText.SetActive(true);
            // Remove last \n at the end
            subText.GetComponentInChildren<TextMeshProUGUI>().text = subTextString.Substring(0, subTextString.Length - 1);
        }
    }

    // TODO these should be moved to the actions themselves if its possible
    private void SetAttackActionLegacy(AttackAction attack) {
        int damage = 0;
        int armour = 0;

        if (attack.attackEffects.Exists(effect => effect.GetType() == typeof(BurnEffect))) {
            buffImage.gameObject.SetActive(true);
            buffImage.sprite = burnImage;
        }

        attack.attackEffects.ForEach(effect => {
            if (effect.GetType() == typeof(DamageEffect)) {
                DamageEffect damageEffect = (DamageEffect)effect;
                damage += damageEffect.damage;
            }
        });

        string stackString = null;

        attack.attackEffects.ForEach(effect => {
            if (effect.GetType() == typeof(DamagePerStackEffect)) {
                DamagePerStackEffect damageEffect = (DamagePerStackEffect)effect;
                stackString = damageEffect.ToDescription();
            }
        });

        string multiplyerString = null;

        attack.attackEffects.ForEach(effect => {
            if (effect.GetType() == typeof(DamageWithMultiplierEffect)) {
                DamageWithMultiplierEffect damageEffect = (DamageWithMultiplierEffect)effect;
                multiplyerString = damageEffect.ToDescription();
            }
        });

        attack.attackEffects.ForEach(effect => {
            if (effect.GetType() == typeof(IncreaseArmour)) {
                IncreaseArmour armourEffect = (IncreaseArmour)effect;
                armour += armourEffect.armourIncrease;
            }
        });

        if (damage > 0) {
            actionText.text = "Attack " + damage;
            //actionImage.sprite = attackImage;
        } else if (armour > 0) {
            actionText.text = "Armour " + armour;
            //actionImage.sprite = shieldImage;
        }

        bool displaySubText = false;
        string subTextString = "";

        if (attack.range > 1) {
            subTextString += "Range " + attack.range + "\n";
            displaySubText = true;
        }

        if (attack.areaOfEffect == AreaOfEffect.CIRCLE || attack.areaOfEffect == AreaOfEffect.AURA) {
            subTextString += "Area " + attack.aoeRange + "\n";
            displaySubText = true;
        }

        if (attack.attackEffects.Exists(effect => effect.GetType() == typeof(SnareEffect))) {
            subTextString += "Snare\n";
            displaySubText = true;
        }

        if (displaySubText) {
            subText.SetActive(true);
            // Remove last \n at the end
            subText.GetComponentInChildren<TextMeshProUGUI>().text = subTextString.Substring(0, subTextString.Length - 1);
        }
    }

    private void SetMoveAction(MoveAction move) {
        //actionImage.sprite = moveImage;
        actionText.text = "Move " + move.distance.ToString();
        //quantityText.text = move.distance.ToString();
    }

    private void SetDrawAction(DrawCardAction draw) {
        //actionImage.sprite = drawImage;
        actionText.text = "Draw " + draw.cardsToDraw.ToString();
        //quantityText.text = draw.cardsToDraw.ToString();
    }
}