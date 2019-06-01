public class SpeedBuff : Buff {
    private int speedIncrease = 1;

    public SpeedBuff(int maxDuration) : base(maxDuration) {
        name = "Speed";
        icon = "buffMomentum";
        flatMod[(int)Stats.SPEED] = 1;
        maxStack = 5;
    }

    public override string GetDescription() {
        return "Speed increased by " + (speedIncrease * stacks) + " for one turn.";
    }
}