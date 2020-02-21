public class Inspired : Buff {

    public Inspired(int maxDuration) : base(maxDuration) {
        name = "Inspired";
        maxStack = 5;
        icon = "icon_inspired";
    }

    public override string GetDescription() {
        return "Attacks I make have advantage.";
    }
}