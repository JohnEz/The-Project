public class Blind : Buff {
    private float percentReduction = 0.5f;

    public Blind() : base(2) {
        name = "Blind";
        isDebuff = true;
        percentMod[(int)Stats.ACCURRACY] = percentReduction;
        icon = "icon_invisible";
    }

    public override string GetDescription() {
        return "Accuracy reduced by " + (percentReduction * 100) + "% for one turn.";
    }
}