public class Haste : Buff {

    public Haste(int maxDuration) : base(maxDuration) {
        name = "Haste";
        flatMod[(int)Stats.AP] = 1;
    }
}