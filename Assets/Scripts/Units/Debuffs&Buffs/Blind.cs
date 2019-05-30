public class Blind : Buff {

    public Blind() : base(2) {
        name = "Blind";
        isDebuff = true;
        percentMod[(int)Stats.ACCURRACY] = 0.5f;
        icon = "icon_invisible";
    }
}