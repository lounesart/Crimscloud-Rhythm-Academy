using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public RhythmPatterns rhythmPatterns;
    public RhythmDisplay rhythmDisplay;
    public AudioSource audioSource;
    public AudioClip beatSound;
    public Button changeRhythmButton;


    private BeatType[][] rhythmPatternArrays;
    private int currentRhythmArrayIndex;
    private int beatIndex;
    private int iterationCount;
    private float changePatternTimer;

    private void Start()
    {
        rhythmPatternArrays = new BeatType[][] { rhythmPatterns.quarterNoteRhythmPattern, rhythmPatterns.eighthNoteRhythmPattern, rhythmPatterns.sixteenthNoteRhythmPattern, rhythmPatterns.quarterNoteRestRhythmPattern };
        currentRhythmArrayIndex = 0;
        beatIndex = 0;
        iterationCount = 0;
        changePatternTimer = 0f;
        SetRhythmPattern();
        changeRhythmButton.onClick.AddListener(SetRhythmPattern);
    }

    private void Update()
    {
        if (rhythmDisplay == null || audioSource == null)
        {
            Debug.LogError("RhythmDisplay or AudioSource is not assigned.");
            return;
        }

        float rhythmTiming = rhythmPatterns.GetBeatTiming(rhythmPatterns.GetBeatType(rhythmPatternArrays[currentRhythmArrayIndex], beatIndex));
        rhythmDisplay.timer += Time.deltaTime;
        changePatternTimer += Time.deltaTime;

        if (rhythmDisplay.timer >= rhythmTiming)
        {
            rhythmDisplay.timer = 0f;
            beatIndex = (beatIndex + 1) % rhythmPatternArrays[currentRhythmArrayIndex].Length;
            if (beatIndex == 0)
            {
                iterationCount++;
                if (iterationCount == 2)
                {
                    iterationCount = 0;
                    changePatternTimer = 0f;
                }
                rhythmDisplay.UpdateRhythmCards(beatIndex);
            }
            audioSource.PlayOneShot(beatSound);
        }

        if (CheckPlayerInput())
        {
            rhythmDisplay.UpdateRhythmCards(beatIndex);
        }
    }

    private bool CheckPlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (rhythmPatternArrays[currentRhythmArrayIndex][beatIndex] == BeatType.QuarterNote)
            {
                beatIndex = (beatIndex + 1) % rhythmPatternArrays[currentRhythmArrayIndex].Length;
                rhythmDisplay.UpdateRhythmCards(beatIndex);
                return true;
            }
            else
            {
                beatIndex = 0;
                iterationCount = 0;
                rhythmDisplay.UpdateRhythmCards(beatIndex);
                rhythmDisplay.timer = 0f;
                return false;
            }
        }

        return false;
    }
    private void SetRhythmPattern()
    {
        currentRhythmArrayIndex = UnityEngine.Random.Range(0, rhythmPatternArrays.Length);
        rhythmDisplay.beatIndex = beatIndex = 0;
        rhythmDisplay.rhythmPatterns.quarterNoteRhythmPattern = rhythmPatternArrays[currentRhythmArrayIndex];
    }
}