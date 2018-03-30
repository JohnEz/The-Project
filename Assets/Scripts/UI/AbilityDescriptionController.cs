using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AbilityDescriptionController : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
 
	}

	public void SetAbility(BaseAbility ability) {
		transform.Find ("Title").GetComponent<Text> ().text = ability.Name;
		transform.Find ("Description").GetComponent<Text> ().text = ability.GetDescription();
	}
	
	public void ShowWindow() {
		gameObject.SetActive (true);
	}

	public void HideWindow() {
		gameObject.SetActive (false);
	}
}

