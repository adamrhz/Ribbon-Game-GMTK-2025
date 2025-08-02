using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public Slider musicSlider; // Reference to the music volume slider
    public Slider sfxSlider;   // Reference to the SFX volume slider
    public AudioMixerGroup SFXMixer;
    public AudioMixerGroup MusicMixer;

    void Start()
    {
        // Initialize sliders with current audio values
        musicSlider.value = (GamePreference.MusicVolume / 100f); // Default to 1.0 if not set
        sfxSlider.value = (GamePreference.SFXVolume / 100f); // Default to 1.0 if not set

        // Add listeners to sliders
        musicSlider.onValueChanged.AddListener(UpdateMusicVolume);
        sfxSlider.onValueChanged.AddListener(UpdateSFXVolume);
        UpdateMusicVolume(musicSlider.value);
        UpdateSFXVolume(sfxSlider.value); // Default to 1.0 if not set
    }

    void UpdateMusicVolume(float value)
    {
        GamePreference.MusicVolume = (int)(value * 100);
        MusicMixer.audioMixer.SetFloat("Music", (value*100) - 80); // Set the music volume in the AudioMixer
    }

    void UpdateSFXVolume(float value)
    {
        GamePreference.SFXVolume = (int)(value * 100);
        SFXMixer.audioMixer.SetFloat("SFX", (value * 100) - 80); // Set the music volume in the AudioMixer
    }
}

