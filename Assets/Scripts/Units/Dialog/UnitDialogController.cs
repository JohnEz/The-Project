using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UnitDialogController : MonoBehaviour {

    float CHANCE_OF_DIALOG = 10;

    TextAsset dialogText;
    UnitDialog myDialog;
    public GameObject dialogPrefab;

    [SerializeField]
    string dialogType;

    public void Initialise(string dialogType) {

    }

    private void Start() {
        LoadDialog(dialogType);
    }

    void LoadDialog(string dialogType) {
        string dialogPath = dialogType != "" ? dialogType : "Template";

        string filePath = "Dialog/Characters/" + dialogPath;
        dialogText = Resources.Load<TextAsset>(filePath);


        myDialog = JsonUtility.FromJson<UnitDialog>(dialogText.ToString());
    }

    public bool ShouldTalk (float chanceMod = 0) {
        float rolledValue = Random.value * 100;
        return rolledValue <= CHANCE_OF_DIALOG + chanceMod;
    }

    public string GetRandomStringFromList(List<string> list) {
        int index = Mathf.RoundToInt(Random.value * (list.Count - 1));
        return list[index];
    }

    public void Intro() {
        DialogAction(myDialog.intro, 100);
    }

    public void Helping() {
        DialogAction(myDialog.helping, 0);
    }

    public void Helped() {
        DialogAction(myDialog.helped, 0);
    }

    public void Attacking() {
        DialogAction(myDialog.attacking, 0);
    }

    public void Attacked() {
        DialogAction(myDialog.attacked, 0);
    }

    public void DialogAction(List<string> dialogList, float chanceMod = 0) {
        if (ShouldTalk()) {
            DisplayDialog(GetRandomStringFromList(dialogList));
        }
    }

    public void DisplayDialog(string dialogText) {
        Transform canvasTransform = transform.Find("unitCanvas(Clone)");

        GameObject speechBubble = Instantiate(dialogPrefab, canvasTransform, false);

        speechBubble.GetComponentInChildren<Text>().text = dialogText;
    }
}
