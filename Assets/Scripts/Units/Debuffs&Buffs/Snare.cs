using UnityEngine;

public class Snare : Buff {

    public Snare(int maxDuration, GameObject persistentFx = null) : base(maxDuration, persistentFx) {
        name = "Snare";
        percentMod[(int)Stats.SPEED] = 0;
        isDebuff = true;
        icon = "buffSnare";
    }

    public override string GetDescription() {
        return "Speed reduced to 0 for one turn.";
    }
}