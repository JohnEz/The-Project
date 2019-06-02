using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Level", menuName = "Level")]
public class LevelObject : ScriptableObject {
    public int index;
    public string levelName = "Level Name";
    public int maxCharacters = 3;
    public string fileName;
    public GameObject mapObject;

    [TextArea]
    public string description = "";

    public List<Objective> playerObjectives;
}