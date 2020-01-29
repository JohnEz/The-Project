using UnityEngine;

public class AttackEffect : ScriptableObject {
    //private Node targetNode;

    public bool targetSelf = false;

    //public Node TargetNode {
    //    get { return targetNode; }
    //}

    public virtual void AbilityEffect(UnitController caster, Node targetNode) {
        targetNode = targetSelf ? caster.myTile.Nodes[0] : targetNode;
    }

    public int PowerModToInt(int power, float mod) {
        return Mathf.RoundToInt(power * mod);
    }
}