using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentFxController : MonoBehaviour {

	[SerializeField]
	AudioClip spawnSfx;
	[SerializeField]
	float spawnSfxDelay = 0f;

	[SerializeField]
	AudioClip loopSfx;
	//[SerializeField]
	//float loopSfxDelay = 0f;

	[SerializeField]
	AudioClip disappearSfx;
	[SerializeField]
	float disappearSfxDelay = 0f;

	// Use this for initialization
	void Start () {
		StartCoroutine (PlayAudioClip (spawnSfx, spawnSfxDelay));
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

	public void Remove(bool withEffects = true) {
		if (withEffects) {
			GetComponent<Animator> ().SetTrigger ("die");
			StartCoroutine (PlayAudioClip (disappearSfx, disappearSfxDelay));
		} else {
			Destroy (gameObject);
		}
	}

	IEnumerator PlayAudioClip(AudioClip audio, float delay) {
		yield return new WaitForSeconds (delay);
        AudioManager.singleton.Play(audio, transform, AudioMixers.SFX);
	}
}
