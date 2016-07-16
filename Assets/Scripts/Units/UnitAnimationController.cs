using UnityEngine;
using System.Collections;

public class UnitAnimationController : MonoBehaviour {

	public bool isWalking = false;
	public bool isAttacking = false;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		Animator anim = GetComponent<Animator> ();
		//TODO This may accidently skip past the last frame and get stuck, must be a better way (TODO is sticking for a frame or two)
		if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attacking") &&
			anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) {
			IsAttacking (false);
			GetComponentInParent<UnitController> ().FinishedAttacking ();
		}
	}

	public void FaceDirection(Vector2 dir) {
		FaceDirection (dir.x, dir.y);
	}

	public void FaceDirection(float x, float y) {
		GetComponent<Animator> ().SetFloat ("dirX", x);
		GetComponent<Animator> ().SetFloat ("dirY", y);
	}

	public void IsWalking(bool walking) {
		isWalking = walking;
		GetComponent<Animator> ().SetBool ("isWalking", walking);
	}

	public void IsAttacking(bool attacking) {
		isAttacking = attacking;
		GetComponent<Animator> ().SetBool ("isAttacking", attacking);
	}

}
