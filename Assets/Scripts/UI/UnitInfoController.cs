using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UnitInfoController : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{

	}

	public void SetUnit(UnitController unit) {
		transform.FindChild ("Name").GetComponent<Text> ().text = unit.GetComponent<UnitClass>().className;
		transform.FindChild ("Description").GetComponent<Text> ().text = getUnitStatsString(unit.myStats);
	}

	public void ShowWindow() {
		gameObject.SetActive (true);
	}

	public void HideWindow() {
		gameObject.SetActive (false);
	}

	public string getUnitStatsString(UnitStats stats) {
		string buffsString = "";
		string debuffsString = "";

		stats.Buffs.ForEach (buff => {
			if (buff.isDebuff) {
				debuffsString += buff.name + ", ";
			} else {
				buffsString += buff.name + ", ";
			}
		});

		if (buffsString.Length > 0) {
			buffsString = buffsString.Substring (0, buffsString.Length-2);
		}

		if (debuffsString.Length > 0) {
			debuffsString = debuffsString.Substring (0, debuffsString.Length-2);
		}

		//return $"HP: {stats.Health}/{stats.MaxHealth}\nPower: {stats.Power}\nArmour: {stats.Armour}\nSpeed: {stats.Speed}\nBuffs:\n{buffsString}\nDebuffs:\n{debuffsString}";
		return "HP: " + stats.Health + "/" + stats.MaxHealth +
		"\nPower: " + stats.Power +
		"\nArmour: " + stats.Armour +
		"\nSpeed: " + stats.Speed +
		"\nBuffs:\n" + buffsString +
		"\nDebuffs:\n" + debuffsString;
	}
}


