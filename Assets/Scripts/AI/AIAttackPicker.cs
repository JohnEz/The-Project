using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIAttackPicker {
    private static AIAttackPicker instance = null;

    private AIAttackPicker() {
    }

    public static AIAttackPicker Instance {
        get {
            if (instance == null) {
                instance = new AIAttackPicker();
            }
            return instance;
        }
    }

    public AITurnPlan GetBestPlan(UnitController myUnit) {
        //Find all the nodes i can attack from
        Dictionary<AIAttackAction, List<Node>> attacksToTiles = FindPossibleAttackNodes(myUnit);

        // Combine the lists and get the value to move to all nodes
        List<Node> allAttackNodes = new List<Node>();
        attacksToTiles.Keys.ToList().ForEach(aiAttackAction => {
            allAttackNodes = allAttackNodes.Union(attacksToTiles[aiAttackAction]).ToList();
        });

        // Find the value to move to all these nodes, return the best ones and their values
        Dictionary<Node, int> nodeValues = AITargetPicker.Instance.GetValueToMoveToNodes(myUnit, allAttackNodes, true);

        //Debug
        // allAttackNodes.ForEach(node => {
        //     node
        //         .GetComponentInChildren<TileHighlighter>()
        //         .DebugSetText(
        //             (nodeValues.Keys.Contains(node) ? 
        //                 nodeValues[node].ToString() :
        //                 "")
        //             .ToString()
        //         );
        // });

        List<AITurnPlan> turnPlans = new List<AITurnPlan>();

        //Find the highest value node to move and attack on
        attacksToTiles.Keys.ToList().ForEach(aiAttackAction => {
            AITurnPlan turnPlan = new AITurnPlan();
            int valueOfAttackAction = aiAttackAction.valueOfAttack;

            List<Node> reachableNodes = attacksToTiles[aiAttackAction].Where(node => nodeValues.Keys.Contains(node)).ToList();

            if (reachableNodes.Count > 0) {

                Dictionary<Node, int> nodeAttackValues = new Dictionary<Node, int>();
                
                reachableNodes.ForEach(node => {
                    int attackValue = CalculateAttackWorth(myUnit, node, aiAttackAction.targetNode, aiAttackAction.attack);
                    nodeAttackValues.Add(node, attackValue);
                });

                Node bestNodeToMoveTo = reachableNodes.Aggregate(
                    (x, y) => nodeValues[x] + nodeAttackValues[x] > nodeValues[y] + nodeAttackValues[y] ? x : y);

                bool canAttackTwice = bestNodeToMoveTo == myUnit.myNode;

                turnPlan.valueOfPlan = (valueOfAttackAction * (canAttackTwice ? 2 : 1)) + nodeValues[bestNodeToMoveTo];
                turnPlan.attack = aiAttackAction;
                turnPlan.targetMoveNode = canAttackTwice ? null : bestNodeToMoveTo;

                turnPlans.Add(turnPlan);
            }
        });

        // Debug
        // turnPlans.ForEach(plan => {
        //    plan.targetMoveNode.GetComponentInChildren<TileHighlighter>().DebugSetText(plan.valueOfPlan.ToString());
        // });

        // Couldnt find a plan
        if (turnPlans.Count < 1) {
            return null;
        }

        return turnPlans.Aggregate((x, y) => x.valueOfPlan > y.valueOfPlan ? x : y);
    }

    // Finds all tiles that can attack an enemy
    public Dictionary<AIAttackAction, List<Node>> FindPossibleAttackNodes(UnitController unit) {
        Dictionary<AIAttackAction, List<Node>> attacksToTiles = new Dictionary<AIAttackAction, List<Node>>();

        UnitManager.singleton.Units.ForEach(otherUnit => {
            unit.myStats.Attacks.ForEach(attackAction => {
                List<Node> attackNodes = new List<Node>();

                if (!attackAction.IsOnCooldown() && attackAction.CanHitUnit(otherUnit.myNode)) {
                    //int valueOfAttack = CalculateAttackWorth(unit, otherUnit.myNode, attackAction);
                    //AIAttackAction aiAttackAction = new AIAttackAction(attackAction, otherUnit.myNode, valueOfAttack);
                    AIAttackAction aiAttackAction = new AIAttackAction(attackAction, otherUnit.myNode, 0);
                    List<Node> nodesToAttackFrom = TileMap.instance.pathfinder.FindAttackableTiles(otherUnit.myNode, attackAction);

                    // This isnt needed anymore but better safe than sorry i guess
                    List<Node> filteredNodesToAttackFrom = nodesToAttackFrom.Where(node => Pathfinder.UnitCanStandOnTile(node, unit.myStats.WalkingType)).ToList();

                    filteredNodesToAttackFrom.ForEach(attackNode => {
                        if (attackNode.myUnit == null || attackNode.myUnit == unit) {
                            attackNodes.Add(attackNode);
                        }
                    });

                    if (attackNodes.Count > 0) {
                        attacksToTiles.Add(aiAttackAction, attackNodes);
                    }
                }
            });
        });

        return attacksToTiles;
    }

    private int CalculateAttackValue(UnitController attacker, Node target, AttackAction attack) {
        if (target.myUnit == null) {
            return 0;
        }

        int value = 0;

        if (target.myUnit != null && attacker.IsAllyOf(target.myUnit)) {
            TileHostility tileHostility = AIInfoCollector.Instance.GetHostilityOfTile(target.myUnit.myPlayer.faction, target);

            int calculatedHealing = attack.GetHealingEstimate();
            int missingHealth = target.myUnit.myStats.MaxHealth - target.myUnit.myStats.Health;
            int effectiveHealing = Mathf.Min(calculatedHealing, missingHealth);

            int calculatedDamageReduction = attack.AppliesStealth() ? tileHostility.heat : attack.GetArmourEstimate() * tileHostility.numberOfAttacks;

            bool savesAlly = target.myUnit.Health <= tileHostility.heat && target.myUnit.Health + effectiveHealing > tileHostility.heat - calculatedDamageReduction;

            value = (effectiveHealing + calculatedDamageReduction) * (savesAlly ? 2 : 1);
        } else {
            int calculatedDamage = attack.GetDamageEstimate() - target.myUnit.myStats.Armour;
            bool killsEnemy = calculatedDamage > target.myUnit.myStats.Health;

            value = calculatedDamage * (killsEnemy ? 2 : 1);
        }

        return value;
    }

    private int CalculateAttackWorth(UnitController attacker, Node start, Node target, AttackAction attack) {
        int valueOfAttack = 0;
        //get all the nodes that target will effect
        List<Node> effectedTiles = TileMap.instance.pathfinder.FindEffectedTiles(start, target, attack);

        //for each node caluclate its effect value
        effectedTiles.ForEach(node => {
            if (node.myUnit == null) {
                return;
            }

            if (attack.CanHitUnit(node)) {
                valueOfAttack += CalculateAttackValue(attacker, target, attack);
            }
        });

        return valueOfAttack;
    }

}