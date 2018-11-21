using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Move Action", menuName = "Card/MoveAction")]
public class MoveAction : CardAction {

    public int distance = 0;
    public Walkable walkingType = Walkable.Walkable;

}
