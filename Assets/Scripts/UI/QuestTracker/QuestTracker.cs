using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestTracker : MonoBehaviour {

    #region Singleton

    private static QuestTracker instance;
    public static QuestTracker Instance { get { return instance; } }

    #endregion Singleton

    public GameObject questPrefab;
    public Transform questsGroup;

    private List<QuestController> quests;

    protected void Awake() {
        if (instance != null) {
            Debug.LogWarning("You have more than one QuestTracker in the scene, please make sure you have only one.");
            return;
        }

        instance = this;
        quests = new List<QuestController>();
    }

    // Use this for initialization
    private void Start() {
    }

    // Update is called once per frame
    private void Update() {
    }

    // TODO allow multiple objectives
    public int AddQuest(string title, Objective objective) {
        GameObject questObject = Instantiate(questPrefab, questsGroup);

        QuestController quest = questObject.GetComponent<QuestController>();

        quest.Initiate(title, objective);

        quests.Add(quest);

        return quests.Count - 1;
    }
}