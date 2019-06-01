using UnityEngine;
using System.Collections;
using DuloGames.UI;

public class BuffIcon : UITooltipShow {
    public static int BUFF_TOOLTIP_WIDTH = 512;
    private Buff buff;

    public void SetBuff(Buff _buff) {
        buff = _buff;
    }

    /// <summary>
    /// Raises the tooltip event.
    /// </summary>
    /// <param name="show">If set to <c>true</c> show.</param>
    public override void OnTooltip(bool show) {
        if (this.buff == null)
            return;

        base.OnTooltip(show);

        // If we are showing the tooltip
        if (show) {
            // Prepare the tooltip lines
            BuffIcon.PrepareTooltip(buff);

            // Anchor to this slot
            UITooltip.AnchorToRect(this.transform as RectTransform);

            // Show the tooltip
            UITooltip.Show();
        } else {
            // Hide the tooltip
            UITooltip.Hide();
        }
    }

    public static void PrepareTooltip(Buff buff) {
        if (buff == null)
            return;

        // Set the tooltip width
        if (UITooltipManager.Instance != null)
            UITooltip.SetWidth(BUFF_TOOLTIP_WIDTH);

        // Set the spell name as title
        UITooltip.AddLine(buff.name, "SpellTitle");

        UITooltip.AddSpacer();

        UITooltip.AddLine(buff.GetDescription(), "SpellDescription");
    }
}