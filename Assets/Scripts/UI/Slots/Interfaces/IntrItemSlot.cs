using UnityEngine;

public interface IntrItemSlot {

    ItemInfo GetItemInfo();

    bool Assign(ItemInfo itemInfo, Object source);

    void Unassign();
}