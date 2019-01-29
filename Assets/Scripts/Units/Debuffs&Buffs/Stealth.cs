public class Stealth : Buff {

    public Stealth(int maxDuration) : base(maxDuration) {
        name = "Stealth";
        maxStack = 1;
        icon = "icon_invisible";
    }
}