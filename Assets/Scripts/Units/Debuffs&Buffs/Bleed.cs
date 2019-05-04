public class Bleed : Buff {

    public Bleed() : base(-1) {
        name = "Bleed";
        maxStack = 5;
        isDebuff = true;
        percentMod[(int)Stats.DAMAGE] = 1 - 0.04f;
        icon = "buffBleed";
    }
}