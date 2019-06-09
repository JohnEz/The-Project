using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBarController : MonoBehaviour {
    public Image hpBar;
    public Image shieldBar;

    public bool displayHPMarkers = true;

    [SerializeField]
    public GameObject hpMarkerPrefab;

    private List<GameObject> hpMarkers = new List<GameObject>();

    private const int HP_MARKER_INTERVAL = 10;

    private float currentMax = 0;
    private float targetHPPercent = 1;
    private float targetShieldPercent = 0;

    private UnitObject targetStats;

    // Use this for initialization
    public void Initialize(UnitObject stats) {
        targetStats = stats;
        currentMax = stats.MaxHealth;
        if (displayHPMarkers) {
            createMarkers(stats.MaxHealth);
        }

        targetStats.onStatChange.AddListener(SetHp);
    }

    public void OnDisable() {
        if (targetStats != null) {
            targetStats.onStatChange.RemoveListener(SetHp);
        }
    }

    // Update is called once per frame
    private void Update() {
        if (targetHPPercent != hpBar.fillAmount) {
            hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, targetHPPercent, 2f * Time.deltaTime);

            float distance = Mathf.Abs(targetHPPercent - hpBar.fillAmount);
            if (distance < 0.005f) {
                hpBar.fillAmount = targetHPPercent;
            }
            UpdateShieldBarPosition();
        }

        if (targetShieldPercent != shieldBar.fillAmount) {
            shieldBar.fillAmount = Mathf.Lerp(shieldBar.fillAmount, targetShieldPercent, 2f * Time.deltaTime);

            float distance = Mathf.Abs(targetShieldPercent - shieldBar.fillAmount);
            if (distance < 0.005f) {
                shieldBar.fillAmount = targetShieldPercent;
            }
        }
    }

    public void SetHp() {
        float health = targetStats.Health;
        float shield = targetStats.Shield;
        float maxHealth = targetStats.MaxHealth;

        targetHPPercent = health / (maxHealth + shield);
        targetShieldPercent = shield / (maxHealth + shield);

        if (displayHPMarkers) {
            if (maxHealth != currentMax + shield) {
                currentMax = maxHealth + shield;
                destroyMarkers();
                createMarkers(maxHealth + shield);
            }
        }
    }

    public void SetHPColor(Color color) {
        hpBar.color = color;
    }

    public void createMarkers(float maxHp) {
        int numberOfMarkers = (int)(maxHp / HP_MARKER_INTERVAL);
        float increment = hpBar.rectTransform.rect.width / (numberOfMarkers);

        for (int i = 0; i < numberOfMarkers - 1; i++) {
            GameObject newMarker = createMarker(i, increment);
            hpMarkers.Add(newMarker);
        }
    }

    public GameObject createMarker(int index, float increment) {
        GameObject newMarker = Instantiate(hpMarkerPrefab);
        Vector3 newPosition = newMarker.transform.position;
        newMarker.transform.SetParent(hpBar.transform, false);
        newPosition.x = Mathf.RoundToInt((index + 1) * increment) - (hpBar.rectTransform.rect.width / 2);
        newMarker.GetComponent<RectTransform>().anchoredPosition = newPosition;
        return newMarker;
    }

    private void UpdateShieldBarPosition() {
        float hpBarEnd = hpBar.rectTransform.rect.width * hpBar.fillAmount;
        shieldBar.rectTransform.anchoredPosition = new Vector3(hpBarEnd - 1, shieldBar.rectTransform.anchoredPosition.y, 0);
    }

    public void destroyMarkers() {
        hpMarkers.ForEach((hpMarker) => {
            Destroy(hpMarker);
        });
        hpMarkers.Clear();
    }
}