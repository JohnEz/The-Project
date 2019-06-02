using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ObjectiveType {
    ANNIHILATION,
    UNIT_SURVIVE,
}

public enum ObjectiveStatus {
    NONE,
    COMPLETE,
    FAILED,
}

public enum GameOutcome {
    NONE,
    WIN,
    LOSS,
}

[Serializable]
public class Objective {

    [Serializable] public class OnObjectiveUpdatedEvent : UnityEvent<ObjectiveStatus> { }

    public OnObjectiveUpdatedEvent onObjectiveUpdated = new OnObjectiveUpdatedEvent();

    public ObjectiveType type;
    public string title;
    public string text;
    public bool optional;

    // TODO this needs to be specific to save unit etc
    public string additionalInfo;

    private ObjectiveStatus status;

    public ObjectiveStatus Status {
        get { return status; }
        set {
            status = value;
            if (onObjectiveUpdated != null) {
                onObjectiveUpdated.Invoke(status);
            }
        }
    }
}

public class ObjectiveManager : MonoBehaviour {
    public static ObjectiveManager instance;

    private Dictionary<Player, List<Objective>> objectives = new Dictionary<Player, List<Objective>>();

    private void Awake() {
        instance = this;
    }

    public void Start() {
        UnitManager.instance.onUnitDie.AddListener(OnUnitDie);
    }

    public void OnDisable() {
        UnitManager.instance.onUnitDie.RemoveListener(OnUnitDie);
    }

    public List<Objective> getObjectives(Player player) {
        return objectives[player];
    }

    public void AddObjective(Player player, Objective objective) {
        objective.Status = ObjectiveStatus.NONE;

        if (!objectives.ContainsKey(player)) {
            List<Objective> newObjectives = new List<Objective>();
            newObjectives.Add(objective);
            objectives.Add(player, newObjectives);
        } else {
            objectives[player].Add(objective);
        }

        if (!player.ai) {
            GUIController.instance.AddObjectiveText(objective);
        }
    }

    public void RemoveObjective() {
    }

    public void OnUnitDie(UnitController deadUnit) {
        UpdateObjectives();
    }

    public void UpdateObjectives() {
        foreach (Player player in objectives.Keys) {
            foreach (Objective objective in objectives[player]) {
                UpdateObjective(player, objective);
            }
        }
    }

    private void UpdateObjective(Player player, Objective objective) {
        switch (objective.type) {
            case ObjectiveType.ANNIHILATION:
                objective.Status = GetAnnihilationStatus(player);
                break;

            case ObjectiveType.UNIT_SURVIVE:
                objective.Status = GetUnitSurviveStatus();
                break;
        }
    }

    public GameOutcome CheckObjectives(Player player) {
        if (objectives.ContainsKey(player)) {
            List<Objective> playerObjectives = objectives[player];
            bool isVictorious = true;
            bool isDeafted = false;

            foreach (Objective objective in playerObjectives) {
                if (objective.Status == ObjectiveStatus.NONE) {
                    if (!objective.optional) {
                        isVictorious = false;
                    }
                } else if (objective.Status == ObjectiveStatus.FAILED) {
                    if (!objective.optional) {
                        isVictorious = false;
                        isDeafted = true;
                    }
                }
            }

            if (isVictorious) {
                return GameOutcome.WIN;
            } else if (isDeafted) {
                return GameOutcome.LOSS;
            }
        }
        return GameOutcome.NONE;
    }

    public bool CheckObjectivesLEGACY(Player player) {
        if (objectives.ContainsKey(player)) {
            List<Objective> playerObjectives = objectives[player];

            foreach (Objective objective in playerObjectives) {
                bool objectiveCompleted;
                switch (objective.type) {
                    case ObjectiveType.ANNIHILATION:
                        objectiveCompleted = Annihilation(player);
                        break;

                    default:
                        objectiveCompleted = true;
                        break;
                }

                if (!objectiveCompleted) {
                    return false;
                }
            }

            return true;
        }
        return false;
    }

    private bool Annihilation(Player player) {
        List<UnitController> units = UnitManager.instance.Units;

        foreach (UnitController unit in units) {
            if (unit.myPlayer.faction != player.faction) {
                return false;
            }
        }

        return true;
    }

    private ObjectiveStatus GetAnnihilationStatus(Player player) {
        // TODO the health check seems hacky to me, what if i want a unit to not die at 0 health
        List<UnitController> aliveEnemyUnits = UnitManager.instance.Units.FindAll(unit => (unit.myPlayer.faction != player.faction && unit.Health > 0));

        return aliveEnemyUnits.Count <= 0 ? ObjectiveStatus.COMPLETE : ObjectiveStatus.NONE;
    }

    private ObjectiveStatus GetUnitSurviveStatus() {
        return ObjectiveStatus.NONE;
    }
}