public class Weaken : Buff {
    private float percentDecrease = 0.1f;

    public Weaken(int maxDuration) : base(maxDuration) {
        name = "Weaken";
        maxStack = 5;
        isDebuff = true;
        percentMod[(int)Stats.POWER] = 1 - percentDecrease;
    }

    public override string GetDescription() {
        return "Power reduced by " + (percentDecrease * 100 * stacks) + "% for one turn.";
    }
}