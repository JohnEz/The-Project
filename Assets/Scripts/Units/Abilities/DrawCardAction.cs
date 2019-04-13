using UnityEngine;

[CreateAssetMenu(fileName = "New Move Action", menuName = "Card/New Draw Action")]
public class DrawCardAction : AbilityAction {
    public int cardsToDraw = 1;
}