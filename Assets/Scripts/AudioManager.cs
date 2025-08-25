using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Timeline;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource1;    // �������� �������� �����
    public AudioSource audioSource2;    // ��������������� �������� �����
    public AudioSource audioSource3;
    public MusicTrack[] tracks;         // ������ ����������� ������
    private int currentTrackIndex = 0;  // ���������� �������� �����

    void Start()
    {
        // ����������� ������ �������� �����
        audioSource1.clip = tracks[currentTrackIndex].clip;
        audioSource1.loop = true; // �������� ���������� ���������������
        audioSource2.loop = true;
        audioSource1.Play();
    }

    public void PlaySoundEffect(AudioClip soundEffect)
    {
        audioSource3.pitch = Random.Range(0.8f, 1.2f);
        audioSource3.PlayOneShot(soundEffect); // ������������� ������ ������ ������� ������
    }

    public void Play()
    {
        // ���������� ������ ������� ����
        if (!audioSource1.isPlaying)
        {
            audioSource1.Play();
        }
    }

    public void StopCurrentTrack()
    {
        // ������������� ������� ����
        audioSource1.Stop();
    }

    public void NextTrack()
    {
        // ������������� �� ��������� ����
        FadeOutAndSwitch();
    }

    void FadeOutAndSwitch()
    {
        // �������� ������� � ��������� �����
        MusicTrack currentTrack = tracks[currentTrackIndex];
        MusicTrack nextTrack = tracks[(currentTrackIndex + 1) % tracks.Length]; // ��������� ���� � ��������� ���������

        // �������� ������� ���������
        StartCoroutine(FadeOut(audioSource1, currentTrack.fadeDuration));

        // ���� ���� �������� ��������, �������� ����������� ���������
        audioSource2.Stop();
        audioSource2.clip = nextTrack.clip;
        audioSource2.volume = 0f;
        audioSource2.Play();

        // ���������� ����������� ��������� ������� ���������
        StartCoroutine(FadeIn(audioSource2, nextTrack.fadeDuration));

        // ������ ������� ������ �����
        currentTrackIndex = (currentTrackIndex + 1) % tracks.Length;

        // ����������� ����������� ������
        SwapSources();
    }

    void SwapSources()
    {
        // ������ ������� ��������� �����
        (audioSource1, audioSource2) = (audioSource2, audioSource1);
    }

    IEnumerator FadeOut(AudioSource source, float duration)
    {
        while (source.volume > 0f)
        {
            source.volume -= Time.deltaTime / duration;
            yield return null;
        }
        source.Stop();
    }

    IEnumerator FadeIn(AudioSource source, float duration)
    {
        while (source.volume < 1f)
        {
            source.volume += Time.deltaTime / duration;
            yield return null;
        }
    }
}