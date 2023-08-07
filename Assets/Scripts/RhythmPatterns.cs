using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum BeatType
{
    QuarterNote,
    EighthNote,
    SixteenthNote,
    QuarterNoteRest
}

public class RhythmPatterns : MonoBehaviour
{
    // The rhythm pattern for each beat type
    public BeatType[] quarterNoteRhythmPattern = { BeatType.QuarterNote };
    public BeatType[] eighthNoteRhythmPattern = { BeatType.EighthNote, BeatType.EighthNote };
    public BeatType[] sixteenthNoteRhythmPattern = { BeatType.SixteenthNote, BeatType.SixteenthNote, BeatType.SixteenthNote, BeatType.SixteenthNote };
    public BeatType[] quarterNoteRestRhythmPattern = { BeatType.QuarterNoteRest };

    // The timing information for each beat type
    public float quarterNoteTiming = 1f;
    public float eighthNoteTiming = 0.5f;
    public float sixteenthNoteTiming = 0.25f;
    public float quarterNoteRestTiming = 1f;

    // The rhythm pattern created by the player
    public BeatType[] playerRhythmPattern;

    public BeatType GetBeatType(BeatType[] rhythmPattern, int index)
    {
        return rhythmPattern[index];
    }

    public float GetBeatTiming(BeatType beatType)
    {
        float timing = 0f;

        switch (beatType)
        {
            case BeatType.QuarterNote:
                timing = quarterNoteTiming;
                break;
            case BeatType.EighthNote:
                timing = eighthNoteTiming;
                break;
            case BeatType.SixteenthNote:
                timing = sixteenthNoteTiming;
                break;
            case BeatType.QuarterNoteRest:
                timing = quarterNoteRestTiming;
                break;
        }

        return timing;
    }

    public void GenerateRhythmPatterns()
    {
        List<BeatType> rhythmPatternList = new List<BeatType>();

        int quarterNoteCount = UnityEngine.Random.Range(1, 5);
        for (int i = 0; i < quarterNoteCount; i++)
        {
            rhythmPatternList.AddRange(quarterNoteRhythmPattern);
        }

        int eighthNoteCount = UnityEngine.Random.Range(1, 5);
        for (int i = 0; i < eighthNoteCount; i++)
        {
            rhythmPatternList.AddRange(eighthNoteRhythmPattern);
        }

        int sixteenthNoteCount = UnityEngine.Random.Range(1, 5);
        for (int i = 0; i < sixteenthNoteCount; i++)
        {
            rhythmPatternList.AddRange(sixteenthNoteRhythmPattern);
        }

        int quarterNoteRestCount = UnityEngine.Random.Range(1, 5);
        for (int i = 0; i < quarterNoteRestCount; i++)
        {
            rhythmPatternList.AddRange(quarterNoteRestRhythmPattern);
        }

        playerRhythmPattern = rhythmPatternList.ToArray();
    }

    public void GenerateRhythmPattern(int patternLength)
    {
        playerRhythmPattern = new BeatType[patternLength];
        for (int i = 0; i < patternLength; i++)
        {
            // Generate a random rhythm pattern using the available beat types
            int randomIndex = UnityEngine.Random.Range(0, 4);
            BeatType beatType = BeatType.QuarterNote;
            switch (randomIndex)
            {
                case 0:
                    beatType = BeatType.QuarterNote;
                    break;
                case 1:
                    beatType = BeatType.EighthNote;
                    break;
                case 2:
                    beatType = BeatType.SixteenthNote;
                    break;
                case 3:
                    beatType = BeatType.QuarterNoteRest;
                    break;
            }
            playerRhythmPattern[i] = beatType;
        }
    }
}