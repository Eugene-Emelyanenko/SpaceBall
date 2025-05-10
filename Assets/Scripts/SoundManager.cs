using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    public float musicVolume = 0.25f;
    public float sfxVolume = 0.5f;
    public static SoundManager Instance { get; private set; }

    public AudioSource sfxSource;
    public AudioSource musicSource;

    public AudioClip clickSound;
    public AudioClip backgroundMusic;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        musicSource.loop = true;

        PlayBackgroundMusic(backgroundMusic);
    }

    public void PlayBackgroundMusic(AudioClip backgroundClip)
    {
        musicSource.Stop();
        musicSource.clip = null;

        musicSource.clip = backgroundClip;
        musicSource.Play();
    }

    public void PlaySfx(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void ChangeMusicVolume(float value)
    {
        musicSource.volume = value;
    }

    public void ChangeSoundVolume(float value)
    {
        sfxSource.volume = value;
    }

    public void Vibrate()
    {
        if (PlayerPrefs.GetFloat("Vibration", 1) > 0)
            Handheld.Vibrate();
    }
}