using System.Collections;
using UnityEngine;

public class PersistentFxController : MonoBehaviour {

    [SerializeField]
    private AudioClip spawnSfx;

    [SerializeField]
    private float spawnSfxDelay = 0f;

    [SerializeField]
    private AudioClip loopSfx;

    //[SerializeField]
    //float loopSfxDelay = 0f;

    [SerializeField]
    private AudioClip disappearSfx;

    [SerializeField]
    private float disappearSfxDelay = 0f;

    // Use this for initialization
    private void Start() {
        StartCoroutine(PlayAudioClip(spawnSfx, spawnSfxDelay));
    }

    // Update is called once per frame
    private void Update() {
        Animator animator = GetComponent<Animator>();
        //TODO This may accidently skip past the last frame and get stuck, must be a better way
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Disappear") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f) {
            Destroy(this.gameObject);
        }
    }

    public void Remove(bool withEffects = true) {
        if (withEffects) {
            GetComponent<Animator>().SetTrigger("die");
            StartCoroutine(PlayAudioClip(disappearSfx, disappearSfxDelay));
        } else {
            Destroy(gameObject);
        }
    }

    private IEnumerator PlayAudioClip(AudioClip audio, float delay) {
        yield return new WaitForSeconds(delay);
        PlayOptions clipOptions = new PlayOptions(audio, transform);
        clipOptions.audioMixer = AudioMixers.SFX;
        AudioManager.singleton.Play(clipOptions);
    }
}