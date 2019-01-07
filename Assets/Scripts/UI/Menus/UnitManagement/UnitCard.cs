using UnityEngine;
using UnityEngine.UI;

public class UnitCard : MonoBehaviour {
    public UnitObject myUnit;

    public Image icon;
    public Text characterName;
    public Text stats;

    private void Start() {
        characterName.text = myUnit.characterName;
        stats.text = GetUnitStatsAsString();
        icon.sprite = myUnit.Icon;
    }

    private string GetUnitStatsAsString() {
        return myUnit.Health + "/" + myUnit.MaxHealth + "\n" + myUnit.Stamina + "/" + myUnit.MaxStamina;
    }
}