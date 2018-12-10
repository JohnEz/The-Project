using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class AIManager : MonoBehaviour {

    public static AIManager singleton;

	List<UnitController> myUnits;

	TileMap myMap;

    private void Awake() {
        singleton = this;
    }

    // Use this for initialization
    void Start () {

	}

	// Update is called once per frame
	void Update () {
		
	}

	public void Initialise(TileMap map) {
		myMap = map;
	}

	public List<UnitController> MyUnits {
		get { return myUnits; }
		set { myUnits = value; }
	}

	// NewTurn is called at the start of each of the AIs turns.
	public IEnumerator NewTurn(int myPlayerId) {
		myUnits = UnitManager.singleton.Units.Where (unit => unit.myPlayer.id == myPlayerId).ToList ();

		foreach (UnitController unit in myUnits) {
			yield return PlanTurn (unit);
		}

		TurnManager.singleton.EndTurn ();
	}

	public List<MovementPath> FindPathsToEnemies(UnitController unit) {
		List<MovementPath> pathsToEnemies = new List<MovementPath> ();

		foreach (UnitController otherUnit in UnitManager.singleton.Units) {
			if (otherUnit.myPlayer.faction != unit.myPlayer.faction) {
				MovementPath pathToEnemy = myMap.pathfinder.FindShortestPathToUnit (unit.myNode, otherUnit.myNode, unit.myStats.walkingType, unit.myPlayer.faction);
				if (pathToEnemy.movementCost != -1) {
					pathsToEnemies.Add (pathToEnemy);
				}
			}
		}

		return pathsToEnemies;
	}

	public Dictionary<AttackAction, List<Node>> GetPotentialAbilityTargets(UnitController unit) {
		Dictionary<AttackAction, List<Node>> potentialAbilityTargets = new Dictionary<AttackAction, List<Node>>();

        unit.myStats.Attacks.ForEach(attackAction => {
            List<Node> attackableTiles = myMap.pathfinder.FindAttackableTiles(unit.myNode, attackAction);

            attackableTiles = attackableTiles.Where(tile => attackAction.CanHitUnit(tile)).ToList();

            potentialAbilityTargets.Add(attackAction, attackableTiles);
        });

        return potentialAbilityTargets;
	}

	public IEnumerator PlanTurn(UnitController unit) {
        Debug.Log("AI - Planning turn");

        bool hasEndedTurn = false;
        bool hasAttacked = false;
        bool hasMoved = false;

        while (!hasEndedTurn) {
            yield return WaitForWaitingForInput();

            Dictionary<AttackAction, List<Node>> potentialAbilityTargets = GetPotentialAbilityTargets(unit);

            AttackAction firstAttack = unit.myStats.Attacks[0];

            // if the first ability has 1 or more available targets
            if (!hasAttacked && potentialAbilityTargets[firstAttack].Count > 0) {
                Debug.Log("AI - attacking tile");
                // gets the first target of the first ability
                Node targetTile = potentialAbilityTargets[firstAttack][0];
                UnitManager.singleton.AttackTile(unit, targetTile, firstAttack);
                hasAttacked = true;
            } else if (!hasMoved && unit.myStats.Speed > 0) {
                // if the unit can move

                // find paths to all enemies
                List<MovementPath> paths = FindPathsToEnemies(unit);

                // if there is a valid path
                if (paths.Count > 0) {
                    // finds the shortest path out of all the paths
                    MovementPath shortestPath = Pathfinder.GetSortestPath(paths);
                    //TODO we need to check if there is a unit on the tile speed away
                    shortestPath.path = shortestPath.path.Take(unit.myStats.Speed).ToList();
                    UnitManager.singleton.SetUnitPath(unit, shortestPath);
                } else {
                    //no options available
                    //end unit turn
                }
                hasMoved = true;
            } else {
                hasEndedTurn = true;
            }

        }
        yield return WaitForWaitingForInput ();

	}

	public IEnumerator WaitForWaitingForInput() {
		return new WaitUntil (() => TurnManager.singleton.CurrentPhase == TurnPhase.WAITING_FOR_INPUT);
	}
}
