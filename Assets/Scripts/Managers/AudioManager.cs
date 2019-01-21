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
    TARGET,
    NONE
}

public class PlayOptions {
    public AudioMixers audioMixer = AudioMixers.MASTER;
    public AudioClip audioClip = null;
    public Transform transform = null;
    public bool loop = false;
    public bool persist = false;
    public float volume = 1f;
    public float pitch = 1f;

    public PlayOptions(AudioClip _audioClip, Transform _transform) {
        audioClip = _audioClip;
        transform = _transform;
    }
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

    private float targetVolume;

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
        } else if (fading == Fade.TARGET) {
            float distanceToFade = Math.Abs(source.volume - targetVolume);
            if (distanceToFade > 0.02f) {
                source.volume = Vector2.Lerp(new Vector2(source.volume, 0), new Vector2(targetVolume, 0), Time.deltaTime).x;
            } else {
                source.volume = targetVolume;
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

    public void FadeTo(float fadeTarget) {
        fading = Fade.TARGET;
        targetVolume = fadeTarget;
    }

    public void Update() {
        UpdateFade();
    }
}

public class AudioManager : MonoBehaviour {
    public static AudioManager singleton;

    public AudioMixer masterMixer;
    public AudioMixerGroup masterMixerGroup;
    public AudioMixerGroup uiMixerGroup;
    public AudioMixerGroup musicMixerGroup;
    public AudioMixerGroup sfxMixerGroup;

    public const string MASTER_VOLUME = "masterVolume";
    public const string MUSIC_VOLUME = "musicVolume";
    public const string SFX_VOLUME = "sfxVolume";

    public Music[] music;

    private Music currentMusic;

    private void Awake() {
        if (singleton != null) {
            Destroy(gameObject);
            return;
        } else {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }

        foreach (Music m in music) {
            m.source = gameObject.AddComponent<AudioSource>();
            m.source.clip = m.intro;

            m.source.outputAudioMixerGroup = musicMixerGroup;
        }

        LoadAudioSettings();
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

    public GameObject Play(PlayOptions options) {
        if (options == null) {
            Debug.LogError("Tried to play audio with no options");
            return null;
        }

        if (options.audioClip == null) {
            Debug.LogError("Tried to play audio with no audio clip");
            return null;
        }

        if (options.transform == null) {
            Debug.LogError("Tried to play audio with no transform");
            return null;
        }

        //Create an empty game object
        GameObject go = CreatePlaySource(options);
        if (go == null) {
            return go;
        }

        if (options.persist) {
            DontDestroyOnLoad(go);
        }
        Destroy(go, options.audioClip.length);

        return go;
    }

    private GameObject CreatePlaySource(PlayOptions options) {
        //Create an empty game object
        GameObject go = new GameObject("Audio: " + options.audioClip.name);
        go.transform.position = options.transform.position;

        //Create the source
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = options.audioClip;
        source.volume = options.pitch;
        source.loop = options.loop;
        source.pitch = options.pitch;

        // Output sound through the sound group or music group
        switch (options.audioMixer) {
            case AudioMixers.MUSIC:
                source.outputAudioMixerGroup = musicMixerGroup;
                break;

            case AudioMixers.UI:
                source.outputAudioMixerGroup = uiMixerGroup;
                break;

            case AudioMixers.SFX:
                source.outputAudioMixerGroup = sfxMixerGroup;
                break;

            default:
                source.outputAudioMixerGroup = masterMixerGroup;
                break;
        }

        source.Play();
        return go;
    }

    public void LowerMusic() {
        currentMusic.FadeTo(0.33f);
    }

    public void RaiseMusic() {
        currentMusic.FadeTo(1);
    }

    private void LoadAudioSettings() {
        float loadedMasterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME);
        masterMixer.SetFloat(MASTER_VOLUME, loadedMasterVolume);

        float loadedMusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME);
        masterMixer.SetFloat(MUSIC_VOLUME, loadedMusicVolume);

        float loadedSFXVolume = PlayerPrefs.GetFloat(SFX_VOLUME);
        masterMixer.SetFloat(SFX_VOLUME, loadedSFXVolume);
    }
}