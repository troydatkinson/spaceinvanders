// Game: Space Invaders Clone
// Author: Troy Atkinson
// Date: 07/01/2018
// Description: Adds GameObject to the DontDestroyOnLoad scene and ensures only one per game. Also doubles up for a quick way of OneShotting sounds.

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Music : MonoBehaviour
{
    private static Music instance;
    private AudioSource audioSource;

    public static bool MusicPlaying
    {
        set
        {
            if (value)
            {
                instance.audioSource.Play();
            }
            else
            {
                instance.audioSource.Stop();
            }
        }
    }

    private void Awake()
    {
        if (instance != null & instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        audioSource = GetComponent<AudioSource>();
        MusicPlaying = true;
        DontDestroyOnLoad(gameObject);
    }

    public static void PlayerOneShot(AudioClip audioClip, float volume = 1f)
    {
        if (audioClip == null)
        {
            Debug.LogError("Audio Clip is NULL.");
        }

        instance.audioSource.PlayOneShot(audioClip, volume);
    }
}