using UnityEngine;

[CreateAssetMenu(fileName = "New Move Action", menuName = "Card/New Move Action")]
public class MoveAction : CardAction {
    public int distance = 1;
    public Walkable walkingType = Walkable.Walkable;
}