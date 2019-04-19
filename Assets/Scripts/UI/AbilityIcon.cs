using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityIcon : MonoBehaviour {
    public Sprite defaultImage;
    public Image iconImage;
    public Image cooldownImage;

    [HideInInspector]
    public Ability displayAbility;

    private float cooldown = 0;
    private float maxCooldown = 0;

    public void Update() {
        if (displayAbility && (displayAbility.Cooldown != cooldown || displayAbility.MaxCooldown != maxCooldown)) {
            UpdateCooldown();
        }
    }

    public void UpdateCooldown() {
        cooldown = displayAbility.Cooldown;
        maxCooldown = displayAbility.MaxCooldown;
        cooldownImage.fillAmount = (float)displayAbility.Cooldown / displayAbility.MaxCooldown;
    }

    public void SetAbility(Ability newAbility) {
        if (!newAbility) {
            SetEmptySlot();
            return;
        }

        displayAbility = newAbility;
        iconImage.sprite = newAbility.icon ? newAbility.icon : defaultImage;
    }

    public void SetEmptySlot() {
        displayAbility = null;
        iconImage.sprite = defaultImage;
        cooldownImage.fillAmount = 0;
    }
}