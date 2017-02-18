using UnityEngine;
using System.Collections;
using System;

public class Buff {

	public int[] flatMod;
	public float[] percentMod;

	public int maxDuration = 1;
	public int duration;

	public bool isDebuff = false;

	public Buff() {
		flatMod = new int[Enum.GetNames(typeof(Stats)).Length];
		percentMod = new float[Enum.GetNames(typeof(Stats)).Length];

		for (int i = 0; i < flatMod.Length; ++i) {
			flatMod [i] = 0;
		}

		for (int i = 0; i < percentMod.Length; ++i) {
			percentMod [i] = 1.0f;
		}

		duration = maxDuration;
	}

}
