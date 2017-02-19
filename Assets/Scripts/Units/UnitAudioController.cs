using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UnitAudioController : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	} 

	public void PlayOneShot(AudioClip sound) {
		GetComponent<AudioSource> ().PlayOneShot (sound);
	}
}
