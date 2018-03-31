using UnityEngine;
using UnityEngine.UI;

public class StaminaBarController : MonoBehaviour {

    public Image staminaBar;

    private float currentMax = 0;
    private float targetPercent = 1;

    // Use this for initialization
    public void Initialize(float maxStamina) {
        staminaBar = transform.Find("staminaBar").GetComponent<Image>();
        currentMax = maxStamina;
    }

    // Update is called once per frame
    void Update() {
        if (targetPercent != staminaBar.fillAmount) {
            staminaBar.fillAmount = Mathf.Lerp(staminaBar.fillAmount, targetPercent, 0.05f);
        }
    }

    public void SetStamina(float currentStamina, float maxStamina) {
        targetPercent = currentStamina / maxStamina;

        if (currentStamina != currentMax) {
            currentMax = currentStamina;
        }
    }

    public void SetHPColor(Color color) {
        staminaBar.color = color;
    }
}
