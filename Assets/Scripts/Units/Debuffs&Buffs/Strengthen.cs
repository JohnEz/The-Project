public class Strengthen : Buff {

    public Strengthen(int maxDuration) : base(maxDuration) {
        name = "Strengthen";
        icon = "buffPower";
        maxStack = 5;
        percentMod[(int)Stats.POWER] = 1.1f;
    }
}