using UnityEngine;
using TMPro;
using DuloGames.UI;
using UnityEngine.UI;

public class AbilityUses : MonoBehaviour {
    private Image usesOverlay;
    private Ability targetAbility;

    public TextMeshProUGUI text;
    public AbilitySlot targetSlot;

    // Use this for initialization
    private void Start() {
        usesOverlay = GetComponent<Image>();
        RemoveUses();
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
            targetAbility = abilityInfo.ability;
            targetAbility.onUsesChange.AddListener(UpdateUses);
            UpdateUses(abilityInfo);
        }
    }

    private void OnSlotUnassign(UISpellSlot spellSlot) {
        UIAbilityInfo abilityInfo = (UIAbilityInfo)spellSlot.GetSpellInfo();
        if (targetAbility != null) {
            targetAbility.onUsesChange.RemoveListener(UpdateUses);
            targetAbility = null;
        }
        RemoveUses();
    }

    public void UpdateUses(UIAbilityInfo abilityInfo) {
        int uses = abilityInfo.RemainingUses;
        if (uses > -1) {
            usesOverlay.enabled = true;
            text.text = string.Format("x{0}", uses);
        } else {
            RemoveUses();
        }
    }

    public void RemoveUses() {
        usesOverlay.enabled = false;
        text.text = "";
    }
}