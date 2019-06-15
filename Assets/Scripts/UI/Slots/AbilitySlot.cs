using UnityEngine;
using System.Collections;
using DuloGames.UI;

public class AbilitySlot : UISpellSlot {

    /// <summary>
    /// Raises the tooltip event.
    /// </summary>
    /// <param name="show">If set to <c>true</c> show.</param>
    public override void OnTooltip(bool show) {
        // Make sure we have spell info, otherwise game might crash
        if (GetSpellInfo() == null)
            return;

        // If we are showing the tooltip
        if (show) {
            UITooltip.InstantiateIfNecessary(this.gameObject);

            // Prepare the tooltip lines
            AbilitySlot.PrepareTooltip(GetSpellInfo());

            // Anchor to this slot
            UITooltip.AnchorToRect(this.transform as RectTransform);

            // Show the tooltip
            UITooltip.Show();
        } else {
            // Hide the tooltip
            UITooltip.Hide();
        }
    }

    public static new void PrepareTooltip(UISpellInfo spellInfo) {
        // Make sure we have spell info, otherwise game might crash
        if (spellInfo == null)
            return;

        UIAbilityInfo abilityInfo = (UIAbilityInfo)spellInfo;

        // Set the tooltip width
        if (UITooltipManager.Instance != null)
            UITooltip.SetWidth(UITooltipManager.Instance.spellTooltipWidth);

        // Set the spell name as title
        UITooltip.AddLine(abilityInfo.Name, "SpellTitle");

        // Spacer
        UITooltip.AddSpacer();

        // Prepare some attributes
        if (abilityInfo.Flags.Has(UISpellInfo_Flags.Passive)) {
            UITooltip.AddLine("Passive", "SpellAttribute");
        } else {
            // Power consumption
            if (abilityInfo.PowerCost > 0f) {
                if (abilityInfo.Flags.Has(UISpellInfo_Flags.PowerCostInPct))
                    UITooltip.AddLineColumn(abilityInfo.PowerCost.ToString("0") + "% Energy", "SpellAttribute");
                else
                    UITooltip.AddLineColumn(abilityInfo.PowerCost.ToString("0") + " Energy", "SpellAttribute");
            }

            // Range
            if (abilityInfo.Range > 0f) {
                if (abilityInfo.Range == 1f)
                    UITooltip.AddLineColumn("Melee range", "SpellAttribute");
                else
                    UITooltip.AddLineColumn(abilityInfo.Range.ToString("0") + " sq range", "SpellAttribute");
            }

            // Cooldown
            UITooltip.AddLineColumn(abilityInfo.MaxCooldown.ToString("0") + " turn cooldown", "SpellAttribute");
        }

        // Set the spell description if not empty
        if (!string.IsNullOrEmpty(abilityInfo.Description)) {
            UITooltip.AddSpacer();
            UITooltip.AddLine(abilityInfo.Description, "SpellDescription");
        }
    }
}