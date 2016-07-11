using UnityEngine;
using System.Collections;

public class UnitAnimationController : MonoBehaviour {

	Animator anim;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void FaceDirection(Vector2 dir) {
		FaceDirection (dir.x, dir.y);
	}

	public void FaceDirection(float x, float y) {
		GetComponent<Animator> ().SetFloat ("dirX", x);
		GetComponent<Animator> ().SetFloat ("dirY", y);
	}

	public void isWalking(bool walking) {
		GetComponent<Animator> ().SetBool ("isWalking", walking);
	}
}
