public class Parry : Buff {
    public AttackAction parryAttack;

    public Parry(int maxDuration, AttackAction attack) : base(maxDuration) {
        name = "Parry";
        maxStack = 5;
        parryAttack = attack;
        icon = "icon_invisible";
    }

    public override string GetDescription() {
        return "Will stop and counter attack the next attack against me.";
    }
}