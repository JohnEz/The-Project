﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBarController : MonoBehaviour {
    public Image hpBar;
    public Image shieldBar;

    [SerializeField]
    public GameObject hpMarkerPrefab;

    private List<GameObject> hpMarkers = new List<GameObject>();

    private const int HP_MARKER_INTERVAL = 10;

    private float currentMax = 0;
    private float targetHPPercent = 1;
    private float targetShieldPercent = 0;

    // Use this for initialization
    public void Initialize(float maxHp) {
        hpBar = transform.Find("hpBar").GetComponent<Image>();
        currentMax = maxHp;
        createMarkers(maxHp);
    }

    // Update is called once per frame
    private void Update() {
        if (targetHPPercent != hpBar.fillAmount) {
            hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, targetHPPercent, 2f * Time.deltaTime);

            float distance = Mathf.Abs(targetHPPercent - hpBar.fillAmount);
            if (distance < 0.01f) {
                hpBar.fillAmount = targetHPPercent;
            }
            UpdateShieldBarPosition();
        }

        if (targetShieldPercent != shieldBar.fillAmount) {
            shieldBar.fillAmount = Mathf.Lerp(shieldBar.fillAmount, targetShieldPercent, 2f * Time.deltaTime);

            float distance = Mathf.Abs(targetShieldPercent - shieldBar.fillAmount);
            if (distance < 0.01f) {
                shieldBar.fillAmount = targetShieldPercent;
            }
        }
    }

    public void SetHP(float currentHp, float maxHp, float shield) {
        targetHPPercent = currentHp / (maxHp + shield);
        targetShieldPercent = shield / (maxHp + shield);

        if (maxHp != currentMax + shield) {
            currentMax = maxHp + shield;
            destroyMarkers();
            createMarkers(maxHp + shield);
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
        newPosition.x = Mathf.RoundToInt((index + 1) * increment);
        newMarker.GetComponent<RectTransform>().anchoredPosition = newPosition;
        return newMarker;
    }

    private void UpdateShieldBarPosition() {
        float hpBarEnd = hpBar.rectTransform.rect.width * hpBar.fillAmount;
        shieldBar.rectTransform.anchoredPosition = new Vector3(hpBarEnd, 0, 0);
    }

    public void destroyMarkers() {
        hpMarkers.ForEach((hpMarker) => {
            Destroy(hpMarker);
        });
        hpMarkers.Clear();
    }
}