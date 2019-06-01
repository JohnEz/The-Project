public class Slow : Buff {
    private int speedReduction = 1;

    public Slow(int maxDuration) : base(maxDuration) {
        name = "Slow";
        icon = "buffMomentum";
        flatMod[(int)Stats.SPEED] = -speedReduction;
        maxStack = 5;
        isDebuff = true;
    }

    public override string GetDescription() {
        return "Speed reduced by " + (speedReduction * stacks) + " for one turn.";
    }
}