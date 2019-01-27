using UnityEngine;

public class SpriteFxController : MonoBehaviour {
    private UnitController myCreator;

    // Use this for initialization
    private void Start() {
    }

    public void Initialise(UnitController _myCreator) {
        myCreator = _myCreator;
    }

    // Update is called once per frame
    private void Update() {
        Animator animator = GetComponent<Animator>();
        //TODO This may accidently skip past the last frame and get stuck, must be a better way
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f) {
            DestroyEffect();
        }
    }

    public void DestroyEffect() {
        if (myCreator != null) {
            myCreator.RemoveEffect(this.gameObject);
        }
        Destroy(this.gameObject);
    }
}