public class Taunt : Buff {
    public UnitController taunter;

    public Taunt(int maxDuration, UnitController _taunter) : base(maxDuration) {
        name = "Taunt";
        icon = "buffArmour";
        isDebuff = true;
        taunter = _taunter;
    }

    public override string GetDescription() {
        return "Forced to attack " + taunter.myStats.characterName + " for one turn.";
    }
}