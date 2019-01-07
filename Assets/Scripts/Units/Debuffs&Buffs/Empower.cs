public class Empower : Buff {

    public Empower(int maxDuration) : base(maxDuration) {
        name = "Empower";
        icon = "buffPower";
        maxStack = 5;
        percentMod[(int)Stats.POWER] = 1.2f;
    }
}