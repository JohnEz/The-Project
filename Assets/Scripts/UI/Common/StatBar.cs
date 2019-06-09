using UnityEngine;
using System.Collections;
using DuloGames.UI;
using TMPro;

public class StatBar : MonoBehaviour {
    public UIProgressBar bar;
    public TextMeshProUGUI currentText;
    public TextMeshProUGUI maxText;
    public TextMeshProUGUI percentageText;

    public void Awake() {
        Clear();
    }

    public void Clear() {
        if (bar != null) {
            bar.fillAmount = 0;
        }

        if (currentText != null) {
            currentText.text = "";
        }

        if (maxText != null) {
            maxText.text = "";
        }

        if (percentageText != null) {
            percentageText.text = "";
        }
    }

    public void UpdateValues(float current, float max) {
        float percent = current / max;

        if (bar != null) {
            bar.fillAmount = percent;
        }

        if (currentText != null) {
            currentText.text = Mathf.CeilToInt(current).ToString();
        }

        if (maxText != null) {
            maxText.text = Mathf.CeilToInt(max).ToString();
        }

        if (percentageText != null) {
            percentageText.text = (percent * 100).ToString("0") + " %";
        }
    }
}