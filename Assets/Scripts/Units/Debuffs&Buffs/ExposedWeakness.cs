public class ExposedWeakness : Buff {

    public ExposedWeakness(int maxDuration) : base(maxDuration) {
        name = "Exposed Weakness";
        maxStack = 5;
        icon = "icon_exposedWeakness";
    }

    public override string GetDescription() {
        return "Attacks against me have advantage.";
    }
}