using System.Collections;
using UnityEngine;

public class SpriteFxController : MonoBehaviour {
    private UnitController myCreator;
    public float selfdestruct_in = 0; // Setting this to 0 means no selfdestruct.

    private void Start() {
        if (selfdestruct_in != 0) {
            StartCoroutine("DestroyEffectWithDelay");
        }
    }

    public void Initialise(UnitController _myCreator) {
        myCreator = _myCreator;
    }

    // Update is called once per frame
    private void Update() {
        Animator animator = GetComponent<Animator>();
        //TODO This may accidently skip past the last frame and get stuck, must be a better way
        if (animator && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f) {
            DestroyEffect();
        }
    }

    public void DestroyEffect() {
        if (myCreator != null) {
            myCreator.RemoveEffect(this.gameObject);
        }
        Destroy(this.gameObject);
    }

    public IEnumerator DestroyEffectWithDelay() {
        yield return new WaitForSeconds(selfdestruct_in);
        DestroyEffect();
    }
}