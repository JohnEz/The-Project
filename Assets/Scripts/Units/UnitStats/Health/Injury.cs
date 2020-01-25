using UnityEngine;
using System.Collections;
using System;

[Serializable]
[CreateAssetMenu(fileName = "New injury", menuName = "Unit/Injury")]
public class Injury : ScriptableObject {
    public Buff effect;
    public string description = "Injury";
}