public class Dodge : Buff {

    public Dodge(int maxDuration) : base(maxDuration) {
        name = "Dodge";
        maxStack = 5;
        icon = "icon_dodge";
    }

    public override string GetDescription() {
        return "Attacks against me have disadvantage.";
    }
}