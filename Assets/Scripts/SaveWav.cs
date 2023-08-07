using System;
using System.IO;
using NAudio.Wave;
using UnityEngine;

public static class SavWav
{
    private const int HeaderSize = 44;

    public static bool Save(string filePath, WaveFormat waveFormat, byte[] audioData)
    {
        if (string.IsNullOrEmpty(filePath) || waveFormat == null || audioData == null || audioData.Length == 0)
            return false;

        string directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        using (var writer = new WaveFileWriter(filePath, waveFormat))
        {
            writer.Write(audioData, 0, audioData.Length);
        }

        return true;
    }

    public static byte[] ConvertAudioClipToByteArray(AudioClip audioClip)
    {
        float[] audioData = new float[audioClip.samples * audioClip.channels];
        audioClip.GetData(audioData, 0);

        byte[] byteArray = new byte[audioData.Length * 2];
        int byteIndex = 0;

        for (int i = 0; i < audioData.Length; i++)
        {
            short shortData = (short)(audioData[i] * 32767);
            byte[] byteArr = BitConverter.GetBytes(shortData);

            byteArray[byteIndex++] = byteArr[0];
            byteArray[byteIndex++] = byteArr[1];
        }

        return byteArray;
    }

    public static WaveFormat CreateWaveFormat(AudioClip audioClip)
    {
        int sampleRate = audioClip.frequency;
        int channels = audioClip.channels;
        int bitsPerSample = 16;

        return new WaveFormat(sampleRate, bitsPerSample, channels);
    }
}
