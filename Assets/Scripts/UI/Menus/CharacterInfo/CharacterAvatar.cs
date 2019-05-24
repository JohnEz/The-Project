using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterAvatar : MonoBehaviour {
    public Image avatarImage;

    public void UpdateAvatar(UnitObject character) {
        if (character == null) {
            avatarImage.sprite = null;
            avatarImage.enabled = false;
            return;
        }

        avatarImage.enabled = true;
        Sprite avatar = character.displayToken.frontSprite;
        avatarImage.sprite = avatar;
        avatarImage.rectTransform.sizeDelta = avatar.rect.size;

        Vector3 currentImagePosition = avatarImage.rectTransform.anchoredPosition;
        avatarImage.rectTransform.anchoredPosition = new Vector3(currentImagePosition.x, -452 + (avatar.rect.height / 2), currentImagePosition.z);
    }
}