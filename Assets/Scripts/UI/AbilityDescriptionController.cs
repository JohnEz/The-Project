using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AbilityDescriptionController : MonoBehaviour
{

	Text title;
	Text description;

	// Use this for initialization
	void Start ()
	{
		title = transform.FindChild ("Title").GetComponent<Text> ();
		description = transform.FindChild ("Description").GetComponent<Text> ();
	}

	public void SetAbility(BaseAbility ability) {
		title.text = ability.Name;
		description.text = ability.Description;
	}
	
	public void ShowDescription() {
		gameObject.SetActive (true);
	}

	public void HideDescription() {
		gameObject.SetActive (false);
	}
}

