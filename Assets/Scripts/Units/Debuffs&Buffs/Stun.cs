public class Stun : Buff {

    public Stun(int maxDuration) : base(maxDuration) {
        name = "Stun";
        isDebuff = true;
    }

    public override string GetDescription() {
        return "Can't perform any actions for one turn.";
    }
}