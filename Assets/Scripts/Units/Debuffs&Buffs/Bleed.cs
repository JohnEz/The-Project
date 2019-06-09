public class Bleed : Buff {
    private float percentDamage = 0.04f;

    public Bleed() : base(-1) {
        name = "Bleed";
        maxStack = 5;
        isDebuff = true;
        percentMod[(int)Stats.DAMAGE] = 1 - percentDamage;
        icon = "icon_bleed";
    }

    public override string GetDescription() {
        return "Dealing " + (percentDamage * 100 * stacks) + "% of max health as damage per turn.";
    }
}