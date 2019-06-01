public class LifeStealBuff : Buff {
    private int lifeStealPercent = 20;

    public LifeStealBuff(int maxDuration) : base(maxDuration) {
        name = "Life Steal";
        //icon = "buffArmour";
        maxStack = 5;
        flatMod[(int)Stats.LIFE_STEAL] = lifeStealPercent;
    }

    public override string GetDescription() {
        return "Regain " + (lifeStealPercent * stacks) + "% of damage dealt as life.";
    }
}