using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    private bool _transitioning = false;
    public float volumeTransitionSpeed = 4f; // Speed at which volume changes
    public AudioSource AudioSource
    {
        get
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
            return audioSource;
        }
    }
    private static MusicPlayer _musicPlayer;
    public static MusicPlayer MPlayer
    {
        get
        {
            return _musicPlayer;
        }
        private set
        {
            _musicPlayer = value;
        }
    }
    void Awake()
    {

        MPlayer = this;
        audioSource = GetComponent<AudioSource>();
    }


    public void PlaySong(string songName, bool loops = false)
    {

        if (!string.IsNullOrEmpty(songName))
        {
            // Load the sprite from Resources folder
            string fullPath = "Soundtrack/" + songName; // Assuming the path is relative to the Resources folder

            AudioClip Song = Resources.Load(fullPath) as AudioClip;
            if (Song != null)
            {
                PlaySong(Song, loops);
            }
        }
    }

    public static void Stop()
    {
        MPlayer.audioSource.Stop();
    }

    public static void Resume()
    {
        MPlayer.audioSource.Play();
    }

    public void PlaySong(AudioClip song, bool loops = false)
    {
        if(song == null)
        {
            Debug.LogWarning("Attempted to play a null song. Please check the song name or path.");
            return;
        }   
        StartCoroutine(TransitioningSong(song, loops));
    }

    public void StopCurrentSong()
    {
        audioSource.Stop();
    }

    private IEnumerator TransitioningSong(AudioClip song, bool loops = false)
    {
        if (audioSource.clip != song)
        {

            while (_transitioning)
            {
                yield return null;
            }

            _transitioning = true;

            // Gradually lower the volume
            while (audioSource.volume > 0)
            {
                audioSource.volume -= volumeTransitionSpeed * Time.deltaTime;
                yield return null;
            }

            // Change the clip and play the new song
            audioSource.Stop();
            audioSource.clip = song;
            audioSource.loop = loops;
            audioSource.Play();

            // Gradually raise the volume
            while (audioSource.volume < 1)
            {
                audioSource.volume += volumeTransitionSpeed * Time.deltaTime;
                yield return null;
            }

            audioSource.volume = 1; // Ensure the volume is exactly 1
            _transitioning = false;
        }
    }



}
