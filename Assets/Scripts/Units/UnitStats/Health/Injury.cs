using UnityEngine;
using System.Collections;
using System;

[Serializable]
[CreateAssetMenu(fileName = "New injury", menuName = "Unit/Injury")]
public class Injury : ScriptableObject {
    public string description = "Injury";
    public bool isActive = false;
}