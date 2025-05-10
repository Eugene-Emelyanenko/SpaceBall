using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [CanBeNull] public TextMeshProUGUI bestResultsText;
    [CanBeNull] public Slider musicSlider;
    [CanBeNull] public Slider sfxSlider;
    [CanBeNull] public Slider vibrationSlider;

    private float musicValue = 0f;
    private float soundValue = 0f;
    private float vibrationValue = 0f;

    void Start()
    {
        GetSettings();
        OnMusicSliderValueChanged(musicValue);
        OnSfxSliderValueChanged(soundValue);
        OnVibrationSliderValueChanged(vibrationValue);

        if (musicSlider != null)
        {
            musicSlider.value = musicValue;       
            musicSlider.onValueChanged.AddListener(OnMusicSliderValueChanged);
        }
        if (sfxSlider != null)
        {
            sfxSlider.value = soundValue;
            sfxSlider.onValueChanged.AddListener(OnSfxSliderValueChanged);
        }
        if (vibrationSlider != null)
        {
            vibrationSlider.value = vibrationValue;
            vibrationSlider.onValueChanged.AddListener(OnVibrationSliderValueChanged);
        }

        if (bestResultsText != null)
        {
            string bestTime = PlayerPrefs.GetString("Time", "0:00");
            int bestScore = PlayerPrefs.GetInt("Score", 0);
            GameManager.UpdateResult(ref bestResultsText, bestTime, bestScore);
        }
    }

    private void OnMusicSliderValueChanged(float value)
    {
        musicValue = value;
        SoundManager.Instance.ChangeMusicVolume(value);
        SaveSettings();
    }

    private void OnSfxSliderValueChanged(float value)
    {
        soundValue = value;
        SoundManager.Instance.ChangeSoundVolume(value);
        SaveSettings();
    }

    private void OnVibrationSliderValueChanged(float value)
    {
        vibrationValue = value;
        SaveSettings();
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("Music", musicValue);
        PlayerPrefs.SetFloat("Sound", soundValue);
        PlayerPrefs.SetFloat("Vibration", vibrationValue);
    }

    public void ResetSettings()
    {
        PlayerPrefs.DeleteKey("Music");
        PlayerPrefs.DeleteKey("Sound");
        PlayerPrefs.DeleteKey("Vibration");
        PlayerPrefs.DeleteKey("Score");
        PlayerPrefs.DeleteKey("Time");

        GetSettings();
        SaveSettings();

        musicSlider.value = musicValue;
        sfxSlider.value = soundValue;
        vibrationSlider.value = vibrationValue;

        SoundManager.Instance.ChangeMusicVolume(musicValue);
        SoundManager.Instance.ChangeSoundVolume(soundValue);
    }

    private void GetSettings()
    {
        musicValue = PlayerPrefs.GetFloat("Music", SoundManager.Instance.musicVolume);
        soundValue = PlayerPrefs.GetFloat("Sound", SoundManager.Instance.sfxVolume);
        vibrationValue = PlayerPrefs.GetFloat("Vibration", 1f);

        PlayerPrefs.Save();
    }
}
