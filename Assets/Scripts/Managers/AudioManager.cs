using System;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioMixers {
    MASTER,
    UI,
    MUSIC,
    SFX,
}

public enum Fade {
    IN,
    OUT,
    NONE
}

[System.Serializable]
public class Music {
    public string name;

    public AudioClip intro;
    public AudioClip mainLoop;
    public AudioClip stinger;

    [HideInInspector]
    public AudioSource source;

    private Fade fading = Fade.NONE;

    private const float FADE_SPEED = 0.5f;

    public void Play(bool fadeIn = true) {
        if (fadeIn) {
            FadeIn();
            source.volume = 0;
        } else {
            source.volume = 1;
        }
        source.clip = mainLoop;
        source.loop = true;
        source.Play();
    }

    public void UpdateFade() {
        if (!source.isPlaying) {
            return;
        }

        if (fading == Fade.IN) {
            if (source.volume < 1) {
                source.volume += FADE_SPEED * Time.deltaTime;
            } else {
                fading = Fade.NONE;
            }
        } else if (fading == Fade.OUT) {
            if (source.volume > 0) {
                source.volume -= FADE_SPEED * Time.deltaTime;
            } else {
                source.Stop();
                fading = Fade.NONE;
            }
        }
    }

    public void FadeIn() {
        fading = Fade.IN;
    }

    public void FadeOut() {
        fading = Fade.OUT;
    }

    public void Update() {
        UpdateFade();
    }
}

public class AudioManager : MonoBehaviour {
    public static AudioManager singleton;

    public AudioMixerGroup masterMixer;
    public AudioMixerGroup uiMixer;
    public AudioMixerGroup musicMixer;
    public AudioMixerGroup sfxMixer;

    public Music[] music;

    private Music currentMusic;

    private void Awake() {
        if (singleton != null) {
            Destroy(gameObject);
        } else {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }

        foreach (Music m in music) {
            m.source = gameObject.AddComponent<AudioSource>();
            m.source.clip = m.intro;

            m.source.outputAudioMixerGroup = musicMixer;
        }
    }

    public void Update() {
        foreach (Music m in music) {
            m.Update();
        }
    }

    public void PlayMusic(string name, bool fadeIn = true) {
        Music m = Array.Find(music, item => item.name == name);
        if (m == null) {
            Debug.LogWarning("Music: " + name + " not found!");
            return;
        }

        if (currentMusic != null) {
            if (m == currentMusic) {
                Debug.LogWarning("Music: " + name + " already playing!");
                return;
            }

            if (fadeIn) {
                currentMusic.FadeOut();
            } else {
                currentMusic.source.Stop();
            }
        }

        currentMusic = m;

        m.Play(fadeIn);
    }

    public GameObject PlayLoop(AudioClip sound, Transform transform, AudioMixers mixer = AudioMixers.MASTER, bool persist = true) {
        return Play(sound, transform, mixer, persist, true);
    }

    public GameObject Play(AudioClip clip, Transform transform, AudioMixers mixer = AudioMixers.MASTER, bool persist = false, bool loop = false) {
        //Create an empty game object
        GameObject go = CreatePlaySource(clip, transform.position, mixer, loop);
        if (go == null) {
            return go;
        }

        if (persist) {
            DontDestroyOnLoad(go);
        }
        Destroy(go, clip.length);

        return go;
    }

    private GameObject CreatePlaySource(AudioClip clip, Vector3 point, AudioMixers mixer, bool loop) {
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
                source.outputAudioMixerGroup = uiMixer;
                break;

            case AudioMixers.SFX:
                source.outputAudioMixerGroup = sfxMixer;
                break;

            default:
                source.outputAudioMixerGroup = masterMixer;
                break;
        }

        source.Play();
        return go;
    }
}