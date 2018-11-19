using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour {

    public AbilityCardBase ability;

    public Text nameText;
    public Image artworkImage;
    public Text descriptionText;

    void Start()
    {
        nameText.text = ability.name;
        artworkImage.sprite = ability.icon;
        descriptionText.text = ability.GetDescription();
    }

}
