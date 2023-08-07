using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using NAudio.Wave;
using NAudio.Dsp;

public class RhythmMelodyExtractor : MonoBehaviour
{
    [SerializeField]
    private Button analyzeButton;

    private string audioFilePath;

    private void Start()
    {
        analyzeButton.onClick.AddListener(AnalyzeAudio);
    }

    private void AnalyzeAudio()
    {
        // Open the file browser and select an audio file
        string selectedFilePath = EditorUtility.OpenFilePanel("Select Audio File", "", "wav,mp3");

        if (!string.IsNullOrEmpty(selectedFilePath))
        {
            audioFilePath = selectedFilePath;
            ExtractRhythmMelody();
        }
    }

    private void ExtractRhythmMelody()
    {
        if (string.IsNullOrEmpty(audioFilePath))
        {
            Debug.LogError("No audio file selected!");
            return;
        }

        // Load the audio file using NAudio
        var audioFileReader = new AudioFileReader(audioFilePath);

        // Create a new audio file path for the rhythm melody
        string rhythmMelodyPath = Path.Combine(Application.persistentDataPath, "RhythmMelody.wav");

        // Rhythm detection variables
        const int sampleRate = 44100;
        const int fftLength = 1024;
        const int bufferSize = fftLength * 2;
        var buffer = new float[bufferSize];
        var fftBuffer = new Complex[fftLength];
        var downbeats = new bool[bufferSize / 2];
        var upbeats = new bool[bufferSize / 2];
        var downbeatThreshold = 0.02f;
        var upbeatThreshold = 0.02f;
        var downbeatPosition = 0;
        var upbeatPosition = 0;

        // Bass and drums extraction variables
        var bassThreshold = 0.01f;
        var drumsThreshold = 0.01f;

        // Calculate the frequency ranges for bass and drums
        var bassRangeStart = (int)(20f / sampleRate * fftLength);
        var bassRangeEnd = (int)(250f / sampleRate * fftLength);
        var drumsRangeStart = (int)(100f / sampleRate * fftLength);
        var drumsRangeEnd = (int)(600f / sampleRate * fftLength);

        // Check for downbeats and upbeats
        while (audioFileReader.Position < audioFileReader.Length)
        {
            var bytesRead = audioFileReader.Read(buffer, 0, bufferSize);
            if (bytesRead == 0)
                break;

            for (int i = 0; i < bytesRead / 2; i++)
            {
                fftBuffer[i].X = buffer[i * 2];
                fftBuffer[i].Y = 0;
            }

            FastFourierTransform.FFT(true, (int)Mathf.Log(fftLength, 2), fftBuffer);

            for (int i = 0; i < fftLength / 2; i++)
            {
                var magnitude = Mathf.Sqrt((float)(fftBuffer[i].X * fftBuffer[i].X + fftBuffer[i].Y * fftBuffer[i].Y));

                if (i >= bassRangeStart && i <= bassRangeEnd && magnitude > bassThreshold)
                {
                    downbeats[downbeatPosition] = true;
                }
                else
                {
                    downbeats[downbeatPosition] = false;
                }

                if (i >= drumsRangeStart && i <= drumsRangeEnd && magnitude > upbeatThreshold && magnitude < drumsThreshold)
                {
                    upbeats[upbeatPosition] = true;
                }
                else
                {
                    upbeats[upbeatPosition] = false;
                }

                downbeatPosition = (downbeatPosition + 1) % (bufferSize / 2);
                upbeatPosition = (upbeatPosition + 1) % (bufferSize / 2);
            }
        }

        // Save the rhythm melody as a separate file
        using (var outputStream = new FileStream(rhythmMelodyPath, FileMode.Create))
        {
            var writer = new WaveFileWriter(outputStream, audioFileReader.WaveFormat);
            audioFileReader.Position = 0;
            audioFileReader.CopyTo(writer);
            writer.Flush();
            writer.Close();
        }

        // Display the status
        Debug.Log("Rhythm melody extracted and saved: " + rhythmMelodyPath);
    }
}
