public class Vitalise : Buff {

    public Vitalise(int maxDuration) : base(maxDuration) {
        name = "Vitalise";
        maxStack = 5;
        flatMod[(int)Stats.HEALING] = 1;
        icon = "icon_regen";
    }
}