using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;

public class BeatInput : MonoBehaviour
{
    public Button beatButton;

    private AudioSampler audioSampler;

    private WaveOutEvent audioOutput;
    private BufferedWaveProvider bufferedWaveProvider;

    private void Start()
    {
        beatButton.onClick.AddListener(BeatButtonOnClick);

        audioSampler = GetComponent<AudioSampler>();

        audioOutput = new WaveOutEvent();
        bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(44100, 2)); // Stereo, 44.1kHz
        audioOutput.Init(bufferedWaveProvider);
    }

    private void OnDestroy()
    {
        // Clean up NAudio resources
        if (audioOutput != null)
        {
            audioOutput.Stop();
            audioOutput.Dispose();
            audioOutput = null;
        }

        if (bufferedWaveProvider != null)
        {
            bufferedWaveProvider.ClearBuffer();
            bufferedWaveProvider = null;
        }
    }

    private void BeatButtonOnClick()
    {
        StopButtonSound();
        RecordBeat();
        PlayButtonSound();
    }

    private void StopButtonSound()
    {
        if (audioOutput != null && audioOutput.PlaybackState == PlaybackState.Playing)
        {
            audioOutput.Stop();
        }
    }

    private void RecordBeat()
    {
        if (audioSampler != null)
        {
            byte[] audioSample = ConvertAudioSampleToByteArray();

            audioSampler.RecordBeat(audioSample);
        }
    }

    private byte[] ConvertAudioSampleToByteArray()
    {
        byte[] byteBuffer = new byte[bufferedWaveProvider.BufferLength];

        int bytesRecorded = bufferedWaveProvider.Read(byteBuffer, 0, byteBuffer.Length);
        Array.Resize(ref byteBuffer, bytesRecorded);

        return byteBuffer;
    }


    private void PlayButtonSound()
    {
        // Creating a short beep sound using the NAudio library
        var beepProvider = new SignalGenerator(bufferedWaveProvider.WaveFormat.SampleRate, bufferedWaveProvider.WaveFormat.Channels);
        beepProvider.Type = SignalGeneratorType.Sin;
        beepProvider.Frequency = 1000;
        beepProvider.Gain = 0.5;

        float durationInSeconds = 0.1f;

        int desiredSampleCount = (int)(durationInSeconds * beepProvider.WaveFormat.SampleRate);

        float[] floatBuffer = new float[desiredSampleCount];
        int samplesRead = beepProvider.Read(floatBuffer, 0, floatBuffer.Length);

        byte[] byteBuffer = new byte[samplesRead * sizeof(float)];
        Buffer.BlockCopy(floatBuffer, 0, byteBuffer, 0, byteBuffer.Length);

        bufferedWaveProvider.ClearBuffer();

        bufferedWaveProvider.DiscardOnBufferOverflow = true;
        bufferedWaveProvider.AddSamples(byteBuffer, 0, byteBuffer.Length);

        if (audioOutput != null && !audioOutput.PlaybackState.Equals(PlaybackState.Playing))
        {
            audioOutput.Play();
        }
    }
}
