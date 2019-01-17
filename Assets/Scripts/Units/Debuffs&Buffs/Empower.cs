﻿public class Empower : Buff {

    public Empower(int maxDuration) : base(maxDuration) {
        name = "Empower";
        icon = "buffPower";
        maxStack = 5;
        flatMod[(int)Stats.POWER] = 1;
    }
}