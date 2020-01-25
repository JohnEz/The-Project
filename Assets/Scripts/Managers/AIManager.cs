using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AITurnPlan {
    public Node targetMoveNode = null;
    public AIAttackAction attack = null;
    public int valueOfPlan = 0;
}

public class AIAttackAction {
    public AttackAction attack = null;
    public Node targetNode = null;
    public int valueOfAttack = 0;

    public AIAttackAction(AttackAction _attack, Node _targetNode, int _valueOfAttack = 0) {
        attack = _attack;
        targetNode = _targetNode;
        valueOfAttack = _valueOfAttack;
    }
}

public class AIManager : MonoBehaviour {
    public static AIManager instance;

    private List<UnitController> myUnits;

    private void Awake() {
        instance = this;
    }

    // Use this for initialization
    private void Start() {
    }

    // Update is called once per frame
    private void Update() {
    }

    public List<UnitController> MyUnits {
        get { return myUnits; }
        set { myUnits = value; }
    }

    // NewTurn is called at the start of each of the AIs turns.
    public void NewTurn(int myPlayerId) {
        TurnManager.instance.EndTurn();
    }
}