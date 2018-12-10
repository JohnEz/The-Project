using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Move Action", menuName = "Card/New Move Action")]
public class MoveAction : CardAction {

    public int distance = 0;
    public Walkable walkingType = Walkable.Walkable;

}
