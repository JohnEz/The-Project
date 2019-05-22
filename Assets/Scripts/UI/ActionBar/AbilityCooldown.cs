using UnityEngine;
using System.Collections;
using DuloGames.UI;
using UnityEngine.UI;
using TMPro;

public class AbilityCooldown : MonoBehaviour {
    private Image cooldownImage;

    public TextMeshProUGUI text;
    public UISpellSlot targetSlot;

    // Use this for initialization
    private void Start() {
        cooldownImage = GetComponent<Image>();
    }

    public void OnEnable() {
        targetSlot.onAssign.AddListener(OnSlotAssign);
        targetSlot.onUnassign.AddListener(OnSlotUnassign);
    }

    public void OnDisable() {
        targetSlot.onAssign.RemoveListener(OnSlotAssign);
        targetSlot.onUnassign.RemoveListener(OnSlotUnassign);
    }

    // Update is called once per frame
    private void Update() {
    }

    private void OnSlotAssign(UISpellSlot spellSlot) {
        UIAbilityInfo abilityInfo = (UIAbilityInfo)spellSlot.GetSpellInfo();
        if (abilityInfo != null) {
            abilityInfo.ability.onCooldownChange.AddListener(UpdateCooldown);
            UpdateCooldown(abilityInfo);
        }
    }

    private void OnSlotUnassign(UISpellSlot spellSlot) {
        UIAbilityInfo abilityInfo = (UIAbilityInfo)spellSlot.GetSpellInfo();
        if (abilityInfo != null) {
            abilityInfo.ability.onCooldownChange.RemoveListener(UpdateCooldown);
        }
        ResetCooldown();
    }

    public void UpdateCooldown(UIAbilityInfo abilityInfo) {
        float cooldown = abilityInfo.Cooldown;
        float maxCooldown = abilityInfo.MaxCooldown;
        if (maxCooldown > 0 && cooldown > 0) {
            cooldownImage.fillAmount = cooldown / maxCooldown;
            text.text = ((int)abilityInfo.Cooldown).ToString();
        } else {
            ResetCooldown();
        }
    }

    public void ResetCooldown() {
        cooldownImage.fillAmount = 0;
        text.text = "";
    }
}