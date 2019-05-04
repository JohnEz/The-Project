public class SpeedBuff : Buff {

    public SpeedBuff(int maxDuration) : base(maxDuration) {
        name = "Speed";
        icon = "buffMomentum";
        flatMod[(int)Stats.SPEED] = 1;
        maxStack = 5;
    }
}