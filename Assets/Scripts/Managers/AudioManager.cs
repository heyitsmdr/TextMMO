using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.Rendering;

public enum SFX
{
    POWER_ON = 0,
    GLITCH = 1,
    BEEP = 2
}

class SoundOptions
{
    public float Volume { get; set; } = 1.0f;
    public bool PlayOneShot { get; set; } = false;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    private List<AudioSource> sfxSources = new List<AudioSource>();

    public AudioClip[] soundEffects; // Assign SFX clips in the Inspector
    public AudioClip backgroundMusic;
    public int sfxPoolSize = 10;

    private AudioSource _musicSource;
    
    void Awake()
    {
        // Ensure only one instance exists (i.e. a singleton)
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("More than one AudioManager in scene.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Get the Audio Source on the object to act as the music player
        _musicSource = GetComponent<AudioSource>();
        
        // Initialize SFX pool
        for (int i = 0; i < sfxPoolSize; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            sfxSources.Add(source);
        }

    }

    public void PlaySFX(SFX sfx, float fadeOutDelay = 0f, float fadeOutDuration = 0f)
    {
        int index = (int)sfx;
        
        if (index >= 0 && index < soundEffects.Length)
        {
            AudioSource availableSource = sfxSources.Find(src => !src.isPlaying);
            if (availableSource != null)
            {
                // Set sound options (or just defaults)
                SoundOptions soundOptions = GetSoundOptions(sfx);
                availableSource.volume = soundOptions.Volume;

                if (soundOptions.PlayOneShot)
                {
                    availableSource.PlayOneShot(soundEffects[index], soundOptions.Volume);
                }
                else
                {
                    availableSource.clip = soundEffects[index];
                    availableSource.Play();
                }

                if (fadeOutDelay > 0f || fadeOutDuration > 0f)
                {
                    StartCoroutine(FadeOutAfterDelay(availableSource, fadeOutDelay, fadeOutDuration));
                }
            }
            else
            {
                Debug.LogWarning("No available AudioSource in the pool!");
            }
        }
    }

    public void PlayMusic()
    {
        if (_musicSource != null && backgroundMusic != null)
        {
            _musicSource.clip = backgroundMusic;
            _musicSource.loop = true;
            _musicSource.Play();
        }
    }

    public IEnumerator PlayMusicAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        PlayMusic();
    }

    private IEnumerator FadeOutAfterDelay(AudioSource source, float seconds, float fadeOutDuration)
    {
        yield return new WaitForSeconds(seconds);

        float startVolume = source.volume;

        while (source.volume > 0)
        {
            source.volume -= startVolume * Time.deltaTime / fadeOutDuration;
            yield return null; // Wait for the next frame
        }

        source.Stop();
        source.volume = startVolume;
    }

    private SoundOptions GetSoundOptions(SFX sound)
    {
        switch (sound)
        {
            case SFX.POWER_ON:
                return new SoundOptions { Volume = 0.6f };
            case SFX.GLITCH:
                return new SoundOptions { Volume = 0.2f };
            case SFX.BEEP:
                return new SoundOptions { Volume = 0.8f, PlayOneShot = true };
            default:
                return new SoundOptions();
        }
    }
}

