using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RhythmDisplay : MonoBehaviour
{
    public RhythmPatterns rhythmPatterns;
    public Image[] rhythmCards = new Image[4];
    public Sprite[] rhythmSprites = new Sprite[4];

    public Button rhythmButton;
    public float timer;
    public int beatIndex;

    void Start()
    {
        rhythmButton.onClick.AddListener(OnRhythmButtonClick);

        timer = 0f;
        beatIndex = 0;

        for (int i = 0; i < rhythmCards.Length; i++)
        {
            rhythmCards[i].enabled = false;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        BeatType beatType = rhythmPatterns.GetBeatType(rhythmPatterns.quarterNoteRhythmPattern, beatIndex);
        float rhythmTiming = rhythmPatterns.GetBeatTiming(beatType);
        if (timer >= rhythmTiming)
        {
            timer = 0f;
            UpdateRhythmCards(beatIndex);
            beatIndex = (beatIndex + 1) % rhythmPatterns.quarterNoteRhythmPattern.Length;
        }
    }

    public void UpdateRhythmCards(int beatIndex)
    {
        for (int i = 0; i < rhythmCards.Length; i++)
        {
            if (i >= rhythmPatterns.quarterNoteRhythmPattern.Length)
            {
                break;
            }

            BeatType beatType = rhythmPatterns.GetBeatType(rhythmPatterns.quarterNoteRhythmPattern, i);
            int spriteIndex = 0;
            switch (beatType)
            {
                case BeatType.QuarterNote:
                    spriteIndex = 0;
                    break;
                case BeatType.EighthNote:
                    spriteIndex = 1;
                    break;
                case BeatType.SixteenthNote:
                    spriteIndex = 2;
                    break;
                case BeatType.QuarterNoteRest:
                    spriteIndex = 3;
                    break;
            }

            rhythmCards[i].sprite = rhythmSprites[spriteIndex];

            if (i == beatIndex)
            {
                rhythmCards[i].enabled = true;
            }
            else
            {
                rhythmCards[i].enabled = false;
            }
        }
    }


    private void OnRhythmButtonClick()
    {
        // Check if the player's input matches the current beat in the rhythm pattern
        if (rhythmPatterns.GetBeatType(rhythmPatterns.quarterNoteRhythmPattern, beatIndex) == BeatType.QuarterNote)
        {
           // do smt
        }
        else
        {

        }
    }

}