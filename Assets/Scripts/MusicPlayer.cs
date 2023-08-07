using System.Collections;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    // The background music to play
    public AudioClip backgroundMusic;

    // The audio source component used to play the music
    private AudioSource audioSource;

    // The current time in the game
    private float currentTime = 0f;

    void Start()
    {
        // Get the audio source component
        audioSource = GetComponent<AudioSource>();

        // Play the background music
        audioSource.clip = backgroundMusic;
        audioSource.Play();

        // Start the game timer
        StartCoroutine(GameTimer());
    }

    IEnumerator GameTimer()
    {
        // Keep track of the time in the game
        while (true)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }
    }
}