public class Vitalise : Buff {

    public Vitalise(int maxDuration, int power) : base(maxDuration) {
        name = "Vitalise";
        maxStack = 5;
        flatMod[(int)Stats.HEALING] = 5 + (int)(power / 2);
    }
}