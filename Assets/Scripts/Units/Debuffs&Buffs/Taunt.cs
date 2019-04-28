public class Taunt : Buff {
    public UnitController taunter;

    public Taunt(int maxDuration, UnitController _taunter) : base(maxDuration) {
        name = "Taunt";
        icon = "buffArmour";
        maxStack = 1;
        taunter = _taunter;
    }
}