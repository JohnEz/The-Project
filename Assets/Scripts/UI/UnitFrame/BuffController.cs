using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class BuffController : MonoBehaviour {
    private Dictionary<string, Sprite[]> buffSprites = new Dictionary<string, Sprite[]>();

    public Transform buffGroup;
    public Transform debuffGroup;

    public GameObject buffPrefab;

    private Dictionary<string, GameObject> buffs;
    private UnitBuffs targetBuffs;

    public void Awake() {
        buffs = new Dictionary<string, GameObject>();
    }

    public void Initialise(UnitBuffs unitBuffs) {
        RemoveListeners();

        targetBuffs = unitBuffs;
        SetupBuffs();
        AddListeners();
    }

    public void OnEnable() {
        AddListeners();
    }

    public void OnDisable() {
        RemoveListeners();
    }

    private void AddListeners() {
        if (targetBuffs == null) {
            return;
        }

        targetBuffs.onAddBuff.AddListener(OnBuffAdded);
        targetBuffs.onRemoveBuff.AddListener(OnBuffRemoved);
    }

    private void RemoveListeners() {
        if (targetBuffs == null) {
            return;
        }

        targetBuffs.onAddBuff.RemoveListener(OnBuffAdded);
        targetBuffs.onRemoveBuff.RemoveListener(OnBuffRemoved);
    }

    public void OnBuffAdded(Buff buff) {
        AddBuff(buff);
    }

    public void OnBuffRemoved(Buff buff) {
        RemoveBuff(buff);
    }

    public void SetupBuffs() {
        Clear();

        if (targetBuffs == null) {
            return;
        }

        targetBuffs.Buffs.ForEach(buff => {
            AddBuff(buff);
        });
    }

    public void AddBuff(Buff addedBuff) {
        // remove if it already existed so we can update
        RemoveBuff(addedBuff);

        Sprite buffSprite = LoadBuffSprite(addedBuff);
        GameObject newBuffIcon = Instantiate(buffPrefab, addedBuff.isDebuff ? debuffGroup : buffGroup, false);

        buffs.Add(addedBuff.name, newBuffIcon);

        if (buffSprite != null) {
            newBuffIcon.GetComponent<Image>().sprite = buffSprite;
        }

        BuffIcon buffIcon = newBuffIcon.GetComponent<BuffIcon>();

        if (buffIcon != null) {
            buffIcon.SetBuff(addedBuff);
        }
    }

    public void RemoveBuff(Buff removedBuff) {
        if (buffs.ContainsKey(removedBuff.name)) {
            Destroy(buffs[removedBuff.name]);
            buffs.Remove(removedBuff.name);
        }
    }

    public void Clear() {
        foreach (GameObject buffIcon in buffs.Values) {
            Destroy(buffIcon);
        }

        buffs.Clear();
    }

    // TODO move to resource manager
    public Sprite LoadBuffSprite(Buff buff) {
        if (!buffSprites.ContainsKey(buff.icon)) {
            buffSprites.Add(buff.icon, Resources.LoadAll<Sprite>("Graphics/UI/InGame/Icons/" + buff.icon));
        }

        //TODO there might be a better way for this but can just use 5 for now (what if only stacks twice but image has more)
        int imageOffset = buff.isDebuff ? buff.maxStack : 0;
        int stackIndex = buff.stacks - 1 + imageOffset;

        stackIndex = Mathf.Clamp(stackIndex, 0, buffSprites[buff.icon].Length - 1);

        return buffSprites[buff.icon][stackIndex];
    }
}