using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentFxController : MonoBehaviour {



	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		Animator animator = GetComponent<Animator> ();
		//TODO This may accidently skip past the last frame and get stuck, must be a better way
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Disappear") && 
			animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f) {
			Destroy (this.gameObject);
		}
	}

	public void Remove() {
		GetComponent<Animator> ().SetTrigger ("die");
	}
}
