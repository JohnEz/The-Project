public class Weaken : Buff {

    public Weaken(int maxDuration) : base(maxDuration) {
        name = "Weaken";
        icon = "buffPower";
        maxStack = 5;
        isDebuff = true;
        percentMod[(int)Stats.POWER] = 0.8f;
    }
}