using UnityEngine;
using System.Collections;
using System;

public class Buff {

	public int[] flatMod;
	public float[] percentMod;

	public int maxDuration = 1;
	public int duration;

	public int maxStack = 1;
	public int stacks = 1;

	public bool isDebuff = false;

	public GameObject persistentFxPrefab;
	public GameObject persistentFx;

	public string name = "Buff Default";
	public string icon = "buffPower";

	public Buff(int _maxDuration, GameObject _persistentFxPrefab = null) {
		flatMod = new int[Enum.GetNames(typeof(Stats)).Length];
		percentMod = new float[Enum.GetNames(typeof(Stats)).Length];

		for (int i = 0; i < flatMod.Length; ++i) {
			flatMod [i] = 0;
		}

		for (int i = 0; i < percentMod.Length; ++i) {
			percentMod [i] = 1.0f;
		}

		persistentFxPrefab = _persistentFxPrefab;

		maxDuration = _maxDuration;
		duration = maxDuration;
	}

	public int GetFlatMod(int index) {
		return flatMod [index] * stacks;
	}

	public float GetPercentMod(int index) {
		float baseMod = percentMod [index] - 1;

		return (baseMod * stacks) + 1;
	}

	public void Remove(bool withEffects = true) {
		if (persistentFx) {
			persistentFx.GetComponent<PersistentFxController> ().Remove (withEffects);
		}
	}

}
