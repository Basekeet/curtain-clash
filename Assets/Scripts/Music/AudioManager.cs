using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Timeline;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource1;    // Основной источник звука
    public AudioSource audioSource2;    // Вспомогательный источник звука
    public MusicTrack[] tracks;         // Массив музыкальных треков
    private int currentTrackIndex = 0;  // Индексация текущего трека

    void Start()
    {
        // Настраиваем первый источник звука
        audioSource1.clip = tracks[currentTrackIndex].clip;
        audioSource1.loop = true; // Включаем постоянное воспроизведение
        audioSource1.Play();
    }

    public void PlaySoundEffect(AudioClip soundEffect)
    {
        audioSource1.PlayOneShot(soundEffect); // Воспроизводим эффект поверх фоновой музыки
    }

    public void Play()
    {
        // Продолжаем играть текущий трек
        if (!audioSource1.isPlaying)
        {
            audioSource1.Play();
        }
    }

    public void StopCurrentTrack()
    {
        // Останавливаем текущий трек
        audioSource1.Stop();
    }

    public void NextTrack()
    {
        // Переключаемся на следующий трек
        FadeOutAndSwitch();
    }

    void FadeOutAndSwitch()
    {
        // Забираем текущий и следующий треки
        MusicTrack currentTrack = tracks[currentTrackIndex];
        MusicTrack nextTrack = tracks[(currentTrackIndex + 1) % tracks.Length]; // Следующий трек с кольцевым переходом

        // Начинаем плавное затухание
        StartCoroutine(FadeOut(audioSource1, currentTrack.fadeDuration));

        // Пока один источник затухает, начинаем проигрывать следующий
        audioSource2.Stop();
        audioSource2.clip = nextTrack.clip;
        audioSource2.volume = 0f;
        audioSource2.Play();

        // Постепенно увеличиваем громкость второго источника
        StartCoroutine(FadeIn(audioSource2, nextTrack.fadeDuration));

        // Меняем текущий индекс трека
        currentTrackIndex = (currentTrackIndex + 1) % tracks.Length;

        // Переключаем источниками ролями
        SwapSources();
    }

    void SwapSources()
    {
        // Меняем местами источники звука
        AudioSource temp = audioSource1;
        audioSource1 = audioSource2;
        audioSource2 = temp;
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