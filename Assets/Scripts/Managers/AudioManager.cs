using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Audio;

public enum AudioMixers {
    MASTER,
    UI,
    MUSIC,
    SFX,
}

public class AudioManager : MonoBehaviour {

    public static AudioManager singleton;

    public AudioMixerGroup masterMixer;
    public AudioMixerGroup uiMixer;
    public AudioMixerGroup musicMixer;
    public AudioMixerGroup sfxMixer;

    void Awake() {
        if (singleton != null) {
            Destroy(gameObject);
        } else {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public AudioSource PlayLoop(AudioClip sound, Transform transform, AudioMixers mixer = AudioMixers.MASTER, bool persist = true) {
        return Play(sound, transform, mixer, persist, true);
    }

    public AudioSource Play(AudioClip clip, Transform transform, AudioMixers mixer = AudioMixers.MASTER, bool persist = false, bool loop = false) {
        //Create an empty game object
        AudioSource source = CreatePlaySource(clip, transform.position, mixer, loop);
        if (persist) {
            DontDestroyOnLoad(source.gameObject);
        }
        Destroy(source.gameObject, clip.length);
        return source;
    }

    private AudioSource CreatePlaySource(AudioClip clip, Vector3 point, AudioMixers mixer, bool loop) {
        if (clip == null) {
            Debug.LogError("Tried to play a null audio clip!");
            return null;
        }

        //Create an empty game object
        GameObject go = new GameObject("Audio: " + clip.name);
        go.transform.position = point;

        //Create the source
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = 1f;
        source.loop = loop;

        // Output sound through the sound group or music group
        switch (mixer) {
            case AudioMixers.MUSIC:
                source.outputAudioMixerGroup = musicMixer;
                break;
            case AudioMixers.UI:
                source.outputAudioMixerGroup = musicMixer;
                break;
            case AudioMixers.SFX:
                source.outputAudioMixerGroup = musicMixer;
                break;
            default:
                source.outputAudioMixerGroup = masterMixer;
                break;
        }

        source.Play();
        return source;
    }

}