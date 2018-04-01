using UnityEngine;
using UnityEngine.UI;

public class StaminaBarController : MonoBehaviour {

    public Image staminaBar;

    private float currentMax = 0;
    private float realFillAmount = 1;
    public float targetPercent = 1;

    private Color maxStaminaColour = new Color(1,1,1);
    private Color zeroStaminaColour = new Color(1, 0.5f, 0.5f);
    private Color negativeStaminaColour = new Color(1, 0, 0);

    // Use this for initialization
    public void Initialize(float maxStamina) {
        staminaBar = transform.Find("staminaBar").GetComponent<Image>();
        currentMax = maxStamina;
    }

    // Update is called once per frame
    void Update() {
        if (targetPercent != staminaBar.fillAmount) {
            float fillAmount = Mathf.Lerp(realFillAmount, targetPercent, 2f * Time.deltaTime);
            realFillAmount = fillAmount;

            bool isNegative = fillAmount < 0;
            Color highColour = maxStaminaColour;

            if (isNegative) {
                
                fillAmount *= -1;
                highColour = negativeStaminaColour;
            }

            Debug.Log(fillAmount);

            staminaBar.fillAmount = fillAmount;
            staminaBar.color = Color.Lerp(zeroStaminaColour, highColour, staminaBar.fillAmount);
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
