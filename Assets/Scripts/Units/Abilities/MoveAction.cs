using UnityEngine;

[CreateAssetMenu(fileName = "New Move Action", menuName = "Ability/New Move Action")]
public class MoveAction : AbilityAction {
    public int distance = 1;
    public Walkable walkingType = Walkable.Walkable;
}