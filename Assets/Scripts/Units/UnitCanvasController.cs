using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UnitCanvasController : MonoBehaviour {

	public GameObject hpBarPrefab;
	public GameObject damageTextPrefab;
	public GameObject actionPointPrefab;
	public GameObject buffIconPrefab;

	HpBarController hpBar;
	Stack<GameObject> actionPoints = new Stack<GameObject>();
	List<GameObject> buffIcons = new List<GameObject> ();

	int myTeam;

	Dictionary<string, Sprite[]> buffSprites = new Dictionary<string, Sprite[]>();

	Color[] teamColours = new Color[]{
		new Color (0, 0.9647f, 1), //blue
		new Color (0.8431f, 0.2f, 0.2f), //red
		new Color (0.7294f, 0.9569f, 0.1176f) //green
	};

	// Use this for initialization
	void Start () {
		UnitController myUnit = GetComponentInParent<UnitController> ();
		myTeam = myUnit.myPlayer.id;
		hpBar = Instantiate (hpBarPrefab).GetComponent<HpBarController>();
		hpBar.transform.SetParent (transform, false);
		hpBar.Initialize (myUnit.myStats.MaxHealth);
		hpBar.SetHPColor (teamColours [myTeam]);
    }

	public void SetActionPoints (int newActionPoints) {
		if (newActionPoints > actionPoints.Count) {
			int difference = newActionPoints - actionPoints.Count;
			for (int i = 0; i < difference; i++) {
				AddActionPoint ();
			}
		} else if (newActionPoints < actionPoints.Count) {
			int difference = actionPoints.Count - newActionPoints;
			for (int i = 0; i < difference; i++) {
				RemoveActionPoint ();
			}
		}
	}

	void AddActionPoint() {
		GameObject newActionPoint = Instantiate (actionPointPrefab);
		Vector3 newPosition = newActionPoint.transform.position;
		newPosition.x = -22 + (actionPoints.Count * 33);
		newActionPoint.transform.position = newPosition;
		newActionPoint.transform.SetParent (transform, false);
		actionPoints.Push(newActionPoint);
	}

	void RemoveActionPoint() {
		if (actionPoints.Count > 0) {
			GameObject removeObject = actionPoints.Pop ();
			Destroy (removeObject);
		}
	}

	// Update is called once per frame
	void Update () {
		UpdateBuffs (GetComponentInParent<UnitStats> ().Buffs);
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
		GameObject newBuffIcon = Instantiate (buffIconPrefab);
		Vector3 newPosition = newBuffIcon.transform.position;
		newPosition.x = -28 + (buffIcons.Count * 25);
		newBuffIcon.transform.position = newPosition;
		newBuffIcon.transform.SetParent (transform, false);
		buffIcons.Add (newBuffIcon);

		if (buffSprite != null) {
			newBuffIcon.GetComponent<Image> ().sprite = buffSprite;
		}
	}

	public void UpdateHP(int currentHP, int maxHP) {
		hpBar.SetHP(currentHP, maxHP);
	}

	public void CreateDamageText(string damage) {
		CreateCombatText(damage, new Color(0.8431f, 0.2f, 0.2f));
	}

	public void CreateHealText(string healing) {
		CreateCombatText(healing, new Color(0.7294f, 0.9569f, 0.1176f));
	}

	public void CreateCombatText(string text, Color color) {
		GameObject newDamageText = Instantiate (damageTextPrefab);
		newDamageText.GetComponent<Text> ().text = text;
		newDamageText.GetComponent<Text> ().color = color;
		newDamageText.transform.SetParent(this.transform);
	}

}
