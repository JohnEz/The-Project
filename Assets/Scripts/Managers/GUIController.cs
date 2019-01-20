using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour {
    public static GUIController singleton;

    public GameObject turnTextPrefab;
    private bool showingTurn = false;
    private GameObject turnText;

    public GameObject errorMessagePrefab;
    public GameObject flyInTextPrefab;

    public GameObject victoryMenuPrefab;
    public GameObject defeatMenuPrefab;

    private List<GameObject> abilityIcons = new List<GameObject>();
    private Dictionary<string, RuntimeAnimatorController> abilityIconControllers = new Dictionary<string, RuntimeAnimatorController>();

    [HideInInspector]
    private GameObject startMenu;

    private GameObject playerHand;
    private GameObject staminaPoints;
    private TextMeshProUGUI objectivesBody;

    private void Awake() {
        singleton = this;
        startMenu = GameObject.Find("StartMenu");
        playerHand = GameObject.Find("Player1Hand");
        playerHand.SetActive(false);
        staminaPoints = GameObject.Find("StaminaPointsFrame");
        staminaPoints.SetActive(false);
    }

    // Use this for initialization
    private void Start() {
    }

    // Update is called once per frame
    private void Update() {
        //TODO this should probably be some sort of timer so there is less of a chance the user can get stuck
        if (showingTurn) {
            if (turnText == null) {
                TurnManager.singleton.FinishStartingTurn();
                showingTurn = false;
            }
        }
    }

    public void BtnStartGame() {
        // TODO animate out
        startMenu.SetActive(false);
        Destroy(startMenu, 1f);
        playerHand.SetActive(true);
        staminaPoints.SetActive(true);
        Invoke("StartGame", 0.5f);
    }

    private void StartGame() {
        GameManager.singleton.StartGame();
    }

    public void GameOver(bool playerWon) {
        AudioManager.singleton.PlayMusic(playerWon ? "Victory" : "Defeat");
        Instantiate(playerWon ? victoryMenuPrefab : defeatMenuPrefab, transform);
    }

    public void StartNewTurn(bool ally, List<Objective> objectives) {
        showingTurn = true;
        turnText = (GameObject)Instantiate(turnTextPrefab, turnTextPrefab.transform.position, Quaternion.identity);
        turnText.transform.SetParent(transform);
        turnText.transform.localPosition = turnTextPrefab.transform.position;
        turnText.transform.localScale = new Vector3(1, 1, 1);
        turnText.transform.Find("AllyTurnImage").gameObject.SetActive(ally);
        turnText.transform.Find("EnemyTurnImage").gameObject.SetActive(!ally);

        if (objectivesBody == null) {
            objectivesBody = transform.Find("Objectives").Find("ObjectivesBody").GetComponent<TextMeshProUGUI>();
        }
        objectivesBody.text = CreateObjectiveText(objectives);

        //TODO THIS SHOULD FIND THE LENGTH OF THE ANIMATION AND NOT BE HARD CODED BUT IM TIRED
        Destroy(turnText, 1.917f);
    }

    public void ClearAbilityIcons() {
        abilityIcons.ForEach((icon) => {
            Destroy(icon.gameObject);
            AbilityDeselected(abilityIcons.IndexOf(icon));
        });
        abilityIcons.Clear();
    }

    public void AbilitySelected(int abilityIndex) {
        if (abilityIcons.Count > abilityIndex) {
            abilityIcons[abilityIndex].GetComponent<Animator>().SetBool("active", true);
        }
    }

    public void AbilityDeselected(int abilityIndex) {
        if (abilityIcons.Count > abilityIndex) {
            abilityIcons[abilityIndex].GetComponent<Animator>().SetBool("active", false);
        }
    }

    private RuntimeAnimatorController LoadRuntimeAnimatorController(string controller) {
        if (!abilityIconControllers.ContainsKey(controller)) {
            abilityIconControllers.Add(controller, Resources.Load<RuntimeAnimatorController>("Graphics/UI/InGame/Icons/" + controller));
        }

        return abilityIconControllers[controller];
    }

    public void UpdateStamina(int newStamina) {
        staminaPoints.GetComponentInChildren<TextMeshProUGUI>().text = newStamina.ToString();
    }

    private string CreateObjectiveText(List<Objective> objectives) {
        string constructedObjectiveText = "";
        foreach (Objective objective in objectives) {
            constructedObjectiveText += objective.text + "\n";
        }
        return constructedObjectiveText;
    }

    public void ShowErrorMessage(string message) {
        GameObject newDamageText = Instantiate(errorMessagePrefab);
        newDamageText.GetComponent<Text>().text = message;
        newDamageText.GetComponent<Text>().color = new Color(0.95f, 0.25f, 0.25f);
        newDamageText.transform.SetParent(this.transform);
    }

    public void HideUI() {
        foreach (Transform child in transform) {
            SlidingElement slider = child.GetComponent<SlidingElement>();

            if (slider != null) {
                slider.CloseMenu(CameraManager.singleton.blendTime * 0.5f);
            }
        }
    }

    public void ShowUI() {
        foreach (Transform child in transform) {
            SlidingElement slider = child.GetComponent<SlidingElement>();

            if (slider != null) {
                slider.OpenMenu(CameraManager.singleton.blendTime * 0.5f);
            }
        }
    }

    public void CreateFlyinText(string message) {
        GameObject newText = Instantiate(flyInTextPrefab, transform);
        newText.GetComponent<TextMeshProUGUI>().text = message;
    }
}