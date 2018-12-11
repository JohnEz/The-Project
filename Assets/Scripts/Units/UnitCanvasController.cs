using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

struct CombatText {
	public string text;
	public Color colour;
}

public class UnitCanvasController : MonoBehaviour {

	const float COMBAT_TEXT_THROTTLE = 0.5f;

	public GameObject hpBarPrefab;
	public GameObject damageTextPrefab;
	public GameObject buffIconPrefab;
    public GameObject staminaBarPrefab;

    StaminaBarController staminaBar;
	HpBarController hpBar;
	List<GameObject> buffIcons = new List<GameObject> ();
	Queue<CombatText> combatTextQueue = new Queue<CombatText>();

	bool canCreateCombatText = true;

    UnitController myUnit;
    int myTeam;

	Dictionary<string, Sprite[]> buffSprites = new Dictionary<string, Sprite[]>();

	Color[] teamColours = new Color[]{
		new Color (0, 0.9647f, 1), //blue
		new Color (0.8431f, 0.2f, 0.2f), //red
		new Color (0.7294f, 0.9569f, 0.1176f) //green
	};

	// Use this for initialization
	void Start () {
		myUnit = GetComponentInParent<UnitController> ();
		myTeam = myUnit.myPlayer.id;
		hpBar = Instantiate (hpBarPrefab, transform, false).GetComponent<HpBarController>();
		hpBar.Initialize (myUnit.myStats.MaxHealth);
		hpBar.SetHPColor (teamColours [myTeam]);

        staminaBar = Instantiate(staminaBarPrefab, transform, false).GetComponent<StaminaBarController>();
        staminaBar.Initialize(myUnit.myStats.MaxStamina);
    }

	// Update is called once per frame
	void Update () {
		//TODO this is really bad to delete and make these buffs each time
		UpdateBuffs (myUnit.myStats.Buffs);
		//TODO there might be a better way to not check for this each time as well
		if (combatTextQueue.Count > 0 && canCreateCombatText) {
			CombatText combatText = combatTextQueue.Dequeue ();
			CreateCombatText (combatText.text, combatText.colour);
		}
	}

	public void UpdateBuffs(List<Buff> buffs) {
		buffIcons.ForEach ((buff) => Destroy (buff));
		buffIcons.Clear ();

		buffs.ForEach ((buff) => AddBuffIcon(buff));
	}

	public Sprite LoadBuffSprite(Buff buff) {
		if (!buffSprites.ContainsKey(buff.icon)) {
			buffSprites.Add (buff.icon, Resources.LoadAll<Sprite> ("Graphics/UI/InGame/Icons/" + buff.icon));
		}

		//TODO there might be a better way for this but can just use 5 for now (what if only stacks twice but image has more)
		int imageOffset = buff.isDebuff ? buff.maxStack : 0;
		int stackIndex = buff.stacks - 1 + imageOffset;

		return buffSprites [buff.icon][stackIndex];
	}

	public void AddBuffIcon(Buff buff) {
		Sprite buffSprite = LoadBuffSprite(buff);
		GameObject newBuffIcon = Instantiate (buffIconPrefab, transform, false);

		Vector3 newPosition = newBuffIcon.transform.localPosition;
		newPosition.x = -28 + (buffIcons.Count * 25);
		newBuffIcon.transform.localPosition = newPosition;

		buffIcons.Add (newBuffIcon);

		if (buffSprite != null) {
			newBuffIcon.GetComponent<Image> ().sprite = buffSprite;
		}
	}

	public void UpdateHP(int currentHP, int maxHP) {
		hpBar.SetHP(currentHP, maxHP);
	}

    public void UpdateStamina(int currentStamina, int maxStamina) {
        staminaBar.SetStamina(currentStamina, maxStamina);
    }

    public void CreateDamageText(string damage) {
		CreateCombatText (damage, new Color (0.8431f, 0.2f, 0.2f));
	}

	public void CreateHealText(string healing) {
		CreateCombatText (healing, new Color (0.7294f, 0.9569f, 0.1176f));
	}

	public void CreateBasicText(string healing) {
		CreateCombatText (healing, new Color (1f, 1f, 1f));
	}

	public void CreateCombatText(string text, Color colour) {
		if (canCreateCombatText) {
			SpawnCombatText (text, colour);
		} else {
			CombatText newText;
			newText.text = text;
			newText.colour = colour;
			combatTextQueue.Enqueue (newText);
		}
	}

	public void SpawnCombatText(string text, Color color) {
		canCreateCombatText = false;
		GameObject newDamageText = Instantiate (damageTextPrefab);
		newDamageText.GetComponent<Text> ().text = text;
		newDamageText.GetComponent<Text> ().color = color;
		newDamageText.transform.SetParent(this.transform);
		StartCoroutine (AllowCreateCombatText ());
	}

	IEnumerator AllowCreateCombatText() {
		yield return new WaitForSeconds (COMBAT_TEXT_THROTTLE);
		canCreateCombatText = true;
	}

}
