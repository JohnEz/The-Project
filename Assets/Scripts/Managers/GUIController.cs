using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour {
    public static GUIController instance;

    public GameObject turnTextPrefab;
    private bool showingTurn = false;
    private GameObject turnText;

    public GameObject errorMessagePrefab;
    public GameObject flyInTextPrefab;

    public GameObject victoryMenuPrefab;
    public GameObject defeatMenuPrefab;

    public GameObject handPrefab;
    public GameObject deckPrefab;

    private List<GameObject> abilityIcons = new List<GameObject>();
    private Dictionary<string, RuntimeAnimatorController> abilityIconControllers = new Dictionary<string, RuntimeAnimatorController>();

    // Pre created ui elements
    public GameObject startMenu;

    public GameObject handLayout;
    public GameObject staminaPoints;
    public TextMeshProUGUI objectiveHeader;
    public TextMeshProUGUI objectivesBody;
    public Button endTurnButton;

    private void Awake() {
        instance = this;
        SetUIActive(false);
        //staminaPoints.SetActive(false);
    }

    // Use this for initialization
    private void Start() {
    }

    // Update is called once per frame
    private void Update() {
        //TODO this should probably be some sort of timer so there is less of a chance the user can get stuck
        if (showingTurn) {
            if (turnText == null) {
                TurnManager.instance.FinishStartingTurn();
                showingTurn = false;
            }
        }
    }

    public void BtnStartGame() {
        // TODO animate out
        startMenu.SetActive(false);
        Destroy(startMenu, 1f);
        SetUIActive(true);
        //staminaPoints.SetActive(true);
        Invoke("StartGame", 0.5f);
    }

    private void StartGame() {
        GameManager.instance.StartGame();
    }

    public void GameOver(bool playerWon) {
        AudioManager.instance.PlayMusic(playerWon ? "Victory" : "Defeat");
        Instantiate(playerWon ? victoryMenuPrefab : defeatMenuPrefab, transform);
    }

    public void StartNewTurn(bool ally, List<Objective> objectives) {
        DisplayNewTurnText(ally);
        UpdateObjectiveText(objectives);
        SetEndTurnEnabled(TurnManager.instance.IsPlayersTurn());
    }

    private void DisplayNewTurnText(bool allyTurn) {
        showingTurn = true;
        turnText = (GameObject)Instantiate(turnTextPrefab, turnTextPrefab.transform.position, Quaternion.identity);
        turnText.transform.SetParent(transform);
        turnText.transform.localPosition = turnTextPrefab.transform.position;
        turnText.transform.localScale = new Vector3(1, 1, 1);
        turnText.transform.Find("AllyTurnImage").gameObject.SetActive(allyTurn);
        turnText.transform.Find("EnemyTurnImage").gameObject.SetActive(!allyTurn);

        //TODO THIS SHOULD FIND THE LENGTH OF THE ANIMATION AND NOT BE HARD CODED BUT IM TIRED
        Destroy(turnText, 1.917f);
    }

    private void UpdateObjectiveText(List<Objective> objectives) {
        if (objectivesBody == null) {
            objectivesBody = transform.Find("Objectives").Find("ObjectivesBody").GetComponent<TextMeshProUGUI>();
        }
        objectivesBody.text = CreateObjectiveText(objectives);
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

    public GameObject CreateHand() {
        return Instantiate(handPrefab, transform.Find("HandLayout"));
    }

    public GameObject CreateDeck() {
        return Instantiate(deckPrefab);
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

    public void SetUIActive(bool isActive) {
        objectiveHeader.gameObject.SetActive(isActive);
        objectivesBody.gameObject.SetActive(isActive);
        endTurnButton.gameObject.SetActive(isActive);
        handLayout.SetActive(isActive);
    }

    public void ShowUI() {
        foreach (Transform child in transform) {
            SlidingElement slider = child.GetComponent<SlidingElement>();

            if (slider != null) {
                slider.OpenMenu(CameraManager.instance.blendTime * 0.33f);
            }
        }
    }

    public void HideUI() {
        foreach (Transform child in transform) {
            SlidingElement slider = child.GetComponent<SlidingElement>();

            if (slider != null && child.gameObject.activeSelf) {
                slider.CloseMenu(CameraManager.instance.blendTime * 0.33f);
            }
        }
    }

    public void SetEndTurnEnabled(bool isEnabled) {
        endTurnButton.interactable = isEnabled;
    }

    public void CreateFlyinText(string message) {
        GameObject newText = Instantiate(flyInTextPrefab, transform);
        newText.GetComponent<TextMeshProUGUI>().text = message;
    }
}