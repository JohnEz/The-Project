public class Strengthen : Buff {
    private float percentIncrease = 0.1f;

    public Strengthen(int maxDuration) : base(maxDuration) {
        name = "Strengthen";
        icon = "icon_strengthen";
        maxStack = 5;
        percentMod[(int)Stats.POWER] = 1 + percentIncrease;
    }

    public override string GetDescription() {
        return "Power increased by " + (percentIncrease * 100 * stacks) + "% for one turn.";
    }
}