public class Burn : Buff {

    public Burn(int maxDuration, int damage) : base(maxDuration) {
        name = "Burn";
        maxStack = 5;
        isDebuff = true;
        flatMod[(int)Stats.DAMAGE] = damage;
        icon = "icon_burn";
    }
}