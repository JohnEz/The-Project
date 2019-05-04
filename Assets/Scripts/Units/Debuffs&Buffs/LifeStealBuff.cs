public class LifeStealBuff : Buff {

    public LifeStealBuff(int maxDuration) : base(maxDuration) {
        name = "Life Steal";
        //icon = "buffArmour";
        maxStack = 5;
        flatMod[(int)Stats.LIFE_STEAL] = 20;
    }
}