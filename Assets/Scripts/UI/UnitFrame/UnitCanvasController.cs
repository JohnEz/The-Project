using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal struct CombatText {
    public string text;
    public Color colour;
}

public class UnitCanvasController : MonoBehaviour {

    //Canvas
    private const float SCALE = 0.05f;

    private const float INVERSE_SCALE = 1 / SCALE;
    private const float HALF_CANVAS_HEIGHT = 200 * SCALE;

    private const float COMBAT_TEXT_THROTTLE = 0.5f;

    public GameObject combatTextPrefab;
    public GameObject buffIconPrefab;

    private HpBarController hpBar;
    private BuffController buffs;
    private TextMeshProUGUI actionPointsText;
    private List<GameObject> buffIcons = new List<GameObject>();
    private Queue<CombatText> combatTextQueue = new Queue<CombatText>();

    public Image telegraph;

    private bool canCreateCombatText = true;

    private UnitController myUnit;
    private int myTeam;

    private Dictionary<string, Sprite[]> buffSprites = new Dictionary<string, Sprite[]>();

    private Color[] teamColours = new Color[] {
        new Color (0, 0.9647f, 1), //blue
        new Color(0.8039f, 0.4039f, 0.2039f), //red
        new Color (0.7294f, 0.9569f, 0.1176f), //green
        //new Color (0.8431f, 0.2f, 0.2f), //red
    };

    // Update is called once per frame
    private void Update() {
        //TODO there might be a better way to not check for this each time as well
        if (combatTextQueue.Count > 0 && canCreateCombatText) {
            CombatText combatText = combatTextQueue.Dequeue();
            CreateCombatText(combatText.text, combatText.colour);
        }

        FaceCamera();
    }

    public void Initialise() {
        myUnit = GetComponentInParent<UnitController>();
        myTeam = myUnit.myPlayer.id;
        Transform unitFrameParent = transform.Find("UnitFrameParent");

        transform.localScale = new Vector3(SCALE, SCALE, SCALE);

        hpBar = unitFrameParent.Find("UnitFrame").GetComponent<HpBarController>();
        buffs = hpBar.GetComponent<BuffController>();

        hpBar.Initialize(myUnit.myStats);
        hpBar.SetHPColor(teamColours[myTeam]);

        buffs.Initialise(myUnit.myStats.buffs);

        float unitFullHeight = (myUnit.myStats.unitHalfHeight * 2);
        float unitHeightToCanvas = (unitFullHeight - HALF_CANVAS_HEIGHT) * INVERSE_SCALE;

        unitFrameParent.localPosition = new Vector3(0, unitHeightToCanvas + unitFrameParent.localPosition.y, 0);
        HideTelegraph();
    }

    public void FaceCamera() {
        transform.LookAt(CameraManager.instance.physicalCamera.transform.position);
        transform.Rotate(0, 180, 0);

        Vector3 currentRotation = transform.rotation.eulerAngles;
        currentRotation.y = CameraManager.instance.GetCameraRotation().y;
        transform.rotation = Quaternion.Euler(currentRotation);
    }

    public void UpdateActionPoints(int ap) {
        if (actionPointsText == null) {
            return;
        }

        actionPointsText.text = ap.ToString();
    }

    public void CreateDamageText(string damage) {
        CreateCombatText(damage, new Color(0.8431f, 0.2f, 0.2f));
    }

    public void CreateHealText(string healing) {
        CreateCombatText(healing, new Color(0.7294f, 0.9569f, 0.1176f));
    }

    public void CreateShieldText(string shield) {
        CreateCombatText(shield, new Color(0.745f, 0.7607f, 0.7961f));
    }

    public void CreateBasicText(string text) {
        CreateCombatText(text, new Color(1f, 1f, 1f));
    }

    public void CreateCombatText(string text, Color colour) {
        if (canCreateCombatText) {
            SpawnCombatText(text, colour);
        } else {
            CombatText newText;
            newText.text = text;
            newText.colour = colour;
            combatTextQueue.Enqueue(newText);
        }
    }

    public void SpawnCombatText(string text, Color color) {
        canCreateCombatText = false;
        GameObject newDamageText = Instantiate(combatTextPrefab, transform);
        newDamageText.GetComponent<TextMeshProUGUI>().text = text;
        newDamageText.GetComponent<TextMeshProUGUI>().color = color;
        StartCoroutine(AllowCreateCombatText());
    }

    private IEnumerator AllowCreateCombatText() {
        yield return new WaitForSeconds(COMBAT_TEXT_THROTTLE);
        canCreateCombatText = true;
    }

    public void HideTelegraph() {
        if (!telegraph || !telegraph.gameObject.activeSelf) {
            return;
        }

        telegraph.gameObject.SetActive(false);
    }

    public void ShowTelegraph(Sprite sprite = null) {
        if (!telegraph) {
            return;
        }

        if (sprite) {
            telegraph.sprite = sprite;
        }

        if (!telegraph.gameObject.activeSelf) {
            telegraph.gameObject.SetActive(true);
        }
    }
}