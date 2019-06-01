using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Level", menuName = "Level")]
public class LevelObject : ScriptableObject {
    public int index;
    public string levelName = "Level Name";
    public int maxCharacters = 3;
    public string fileName;
    public GameObject mapObject;
}