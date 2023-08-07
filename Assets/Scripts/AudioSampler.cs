using UnityEngine;
using UnityEngine.UI;
using NAudio.Wave;
using System.Collections.Generic;
using System.IO;
using System;
using TMPro;

public class AudioSampler : MonoBehaviour
{
    public Button recordButton;
    public Button saveButton;
    public Button discardButton;
    public Button replayButton;

    private List<float> recordedBeats;
    private WaveInEvent audioInput;
    private MemoryStream audioStream; // Stream to store recorded audio data
    private WaveFileWriter writer; // NAudio writer for saving the recorded beats
    private WaveOutEvent audioOutput;
    private bool isRecording = false;

    private void Start()
    {
        recordedBeats = new List<float>();

        recordButton.onClick.AddListener(RecordButtonOnClick);
        saveButton.onClick.AddListener(SaveButtonOnClick);
        discardButton.onClick.AddListener(DiscardButtonOnClick);
        replayButton.onClick.AddListener(ReplayButtonOnClick);
    }

    private void OnDestroy()
    {
        if (audioOutput != null)
        {
            audioOutput.Stop();
            audioOutput.Dispose();
            audioOutput = null;
        }

        if (writer != null)
        {
            writer.Dispose();
            writer = null;
        }
    }

    public void RecordBeat(byte[] audioSample)
    {
        foreach (var sample in audioSample)
        {
            recordedBeats.Add(sample);
        }
    }

    private void StartRecording()
    {
        if (!isRecording)
        {
            audioInput = new WaveInEvent();
            audioInput.WaveFormat = new WaveFormat(44100, 1); // Sample rate: 44100, Mono

            audioInput.DataAvailable += AudioInputOnDataAvailable;
            audioInput.RecordingStopped += AudioInputOnRecordingStopped;

            // Storing recorded audio data
            audioStream = new MemoryStream();

            writer = new WaveFileWriter(audioStream, audioInput.WaveFormat);

            // Start recording
            audioInput.StartRecording();
            isRecording = true;
            recordButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stop Recording";
        }
    }

    private void StopRecording()
    {
        if (isRecording)
        {
            if (audioInput == null)
            {
                return;
            }

            audioInput.StopRecording();
            isRecording = false;
            recordButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start Recording";

            
        }
    }

    private void RecordButtonOnClick()
    {
        if (audioInput == null)
        {
            StartRecording();
        }
        else
        {
            StopRecording();
        }
    }

    private void AudioInputOnDataAvailable(object sender, WaveInEventArgs waveInEventArgs)
    {
        writer.Write(waveInEventArgs.Buffer, 0, waveInEventArgs.BytesRecorded);
        audioStream.Write(waveInEventArgs.Buffer, 0, waveInEventArgs.BytesRecorded);
    }

    private void AudioInputOnRecordingStopped(object sender, StoppedEventArgs stoppedEventArgs)
    {
        

        audioStream.Seek(0, SeekOrigin.Begin);

        // Making a copy of the audio stream
        MemoryStream copiedStream = new MemoryStream();
        audioStream.CopyTo(copiedStream);
        copiedStream.Seek(0, SeekOrigin.Begin);

        // New WAV file with the recorded beat samples
        string savePath = Path.Combine(Application.persistentDataPath, "recorded_beats.wav");
        using (WaveFileWriter saveWriter = new WaveFileWriter(savePath, audioInput.WaveFormat))
        {
            byte[] buffer = new byte[copiedStream.Length];
            copiedStream.Read(buffer, 0, buffer.Length);
            saveWriter.Write(buffer, 0, buffer.Length);
        }

        Debug.Log("Recorded beats saved to: " + savePath);

        // Reseting recorded beats list and release the memory stream
        recordedBeats.Clear();
        writer.Flush();
        writer.Dispose();
        writer = null;

        audioStream.Dispose();
        audioStream = null;


  

        // Cleaning up audio input device
        audioInput.Dispose();
        audioInput = null;


    }

    private void SaveButtonOnClick()
    {
        if (recordedBeats.Count > 0)
        {
            // Converting the recorded beats list to a byte array
            byte[] byteArray = new byte[recordedBeats.Count * sizeof(float)];
            Buffer.BlockCopy(recordedBeats.ToArray(), 0, byteArray, 0, byteArray.Length);

            // Creating a new WAV file with the recorded beat samples
            string savePath = Path.Combine(Application.persistentDataPath, "recorded_beats.wav");
            using (WaveFileWriter writer = new WaveFileWriter(savePath, new WaveFormat(44100, 1))) // Sample rate: 44100, Mono
            {
                writer.Write(byteArray, 0, byteArray.Length);
            }

            Debug.Log("Recorded beats saved to: " + savePath);
        }
    }

    private void DiscardButtonOnClick()
    {
        if (audioInput == null)
        {
            return;
        }

        StopRecording();
        recordedBeats.Clear();
    }
    private void ReplayButtonOnClick()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "recorded_beats.wav");
        if (File.Exists(filePath))
        {
            try
            {
                WaveFileReader audioFileReader = new WaveFileReader(filePath);

                audioOutput = new WaveOutEvent();
                audioOutput.Init(audioFileReader);

                audioOutput.Play();
            }
            catch (FormatException ex)
            {
                Debug.LogError("Invalid WAV file format: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("Recorded beats file not found: " + filePath);
        }
    }
}
