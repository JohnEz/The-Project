using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

public class UnitBuffs {

    [Serializable] public class OnAddBuffEvent : UnityEvent<Buff> { }

    [Serializable] public class OnRemoveBuffEvent : UnityEvent<Buff> { }

    public OnAddBuffEvent onAddBuff = new OnAddBuffEvent();
    public OnRemoveBuffEvent onRemoveBuff = new OnRemoveBuffEvent();

    public List<Buff> Buffs { get; set; } = new List<Buff>();

    public void Clear() {
        RemoveBuffs(Buffs);
    }

    public List<Buff> GetBuffs() {
        return Buffs;
    }

    public Buff FindOldestBuff(bool debuff) {
        return Buffs.Find(buff => buff.isDebuff = debuff);
    }

    public Buff FindNewestBuff(bool debuff) {
        return Buffs.FindLast(buff => buff.isDebuff == debuff);
    }

    public Buff FindBuff(string name) {
        return Buffs.Find(buff => buff.name == name);
    }

    public List<Buff> FindBuffs(string name) {
        return Buffs.FindAll(buff => buff.name == name);
    }

    public List<Buff> FindBuffs(bool debuff) {
        return Buffs.FindAll(buff => buff.isDebuff == debuff);
    }

    public void RemoveBuff(Buff buff, bool withEffects = true) {
        buff.Remove(withEffects);
        Buffs.Remove(buff);
        OnRemoveBuff(buff);
    }

    public bool ApplyBuff(Buff newBuff) {
        Buff currentBuff = FindBuff(newBuff.name);

        if (currentBuff != null) {
            int newDuration = Mathf.Max(currentBuff.duration, newBuff.maxDuration);
            int newStacks = Mathf.Min(currentBuff.stacks + 1, currentBuff.maxStack);

            newBuff.maxDuration = newDuration;
            newBuff.duration = newDuration;
            newBuff.stacks = newStacks;

            RemoveBuff(currentBuff, false);
        }

        Buffs.Add(newBuff);
        OnAddBuff(newBuff);

        return true;
    }

    public void RemoveBuffs(List<Buff> buffsToRemove, bool withEffects = true) {
        buffsToRemove.ForEach(buff => RemoveBuff(buff, withEffects));
    }

    public void NewTurn() {
        List<Buff> buffsToRemove = new List<Buff>();

        Buffs.ForEach((buff) => {
            buff.duration--;
            if (buff.maxDuration != -1 && buff.duration <= 0) {
                buffsToRemove.Add(buff);
            }
        });

        buffsToRemove.ForEach((buff) => {
            RemoveBuff(buff);
        });
    }

    private void OnAddBuff(Buff buff) {
        if (this.onAddBuff != null) {
            this.onAddBuff.Invoke(buff);
        }
    }

    private void OnRemoveBuff(Buff buff) {
        if (this.onRemoveBuff != null) {
            this.onRemoveBuff.Invoke(buff);
        }
    }
}