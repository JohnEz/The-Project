using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

    public UnitObject myUnit;

    public Text nameText;
    public Image icon;

    public void OnDrop(PointerEventData eventData) {
        UnitCard unitCard = eventData.pointerDrag.GetComponent<UnitCard>();

        if (unitCard != null) {
            if (myUnit != null) {
                //MatchDetails.PlayerRoster.Remove(myUnit);
            }

            myUnit = unitCard.myUnit;
            Destroy(unitCard.gameObject);
            LoadUnit(myUnit);
            //MatchDetails.PlayerRoster.Add(myUnit);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        
    }

    public void OnPointerExit(PointerEventData eventData) {
        
    }

    void LoadUnit(UnitObject unit) {
        nameText.text = unit.characterName;
        icon.sprite = unit.Icon;
    }

}
