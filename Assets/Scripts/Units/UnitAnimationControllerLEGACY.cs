using UnityEngine;

public class UnitAnimationControllerLEGACY : MonoBehaviour {
    public bool isWalking = false;
    public bool isAttacking = false;
    public bool attackHasLanded = false;
    public bool facingRight = true;
    public bool isSelected = false;

    // Use this for initialization
    private void Start() {
    }

    // Update is called once per frame
    private void Update() {
        Animator anim = GetComponent<Animator>();
        //TODO This may accidently skip past the last frame and get stuck, must be a better way (TODO is sticking for a frame or two)
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attacking") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f) {
            IsAttacking(false);
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Death") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f) {
            DestroyParentUnit();
        }
    }

    public void AttackHit() {
        //deal damage etc
        attackHasLanded = true;
    }

    public void FaceDirection(Vector2 dir) {
        FaceDirection(dir.x, dir.y);
    }

    public void FaceDirection(float x, float y) {
        GetComponent<Animator>().SetFloat("dirX", x);
        GetComponent<Animator>().SetFloat("dirY", y);
        if (x > 0) {
            facingRight = true;
            transform.localScale = new Vector3(1, 1, 1);
        } else if (x < 0) {
            facingRight = false;
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void IsWalking(bool walking) {
        isWalking = walking;
        GetComponent<Animator>().SetBool("isWalking", walking);
    }

    public void IsAttacking(bool attacking) {
        isAttacking = attacking;
        attackHasLanded = !attacking;
        GetComponent<Animator>().SetBool("isAttacking", attacking);
    }

    public void IsSelected(bool selected) {
        isSelected = selected;
        GetComponent<Animator>().SetBool("isSelected", selected);
    }

    public void PlayHitAnimation() {
        GetComponent<Animator>().SetTrigger("hit");
    }

    public void PlayDeathAnimation() {
        GetComponent<Animator>().SetTrigger("die");
    }

    public void DestroyParentUnit() {
        GetComponentInParent<UnitController>().DestroySelf();
    }
}