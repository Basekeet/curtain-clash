using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public struct MusicTrack
{
    public AudioClip clip;       // аудиофайл
    public float fadeDuration;   // длительность плавного затухания (секунды)
}
