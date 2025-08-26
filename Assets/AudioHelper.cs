using UnityEngine;
using DG.Tweening;
using Mirror;

public static class AudioHelper
{
    public static void PlayOneshot(AudioClip clip, AudioSource source)
    {
        if (clip && source)
        {
            source.DOPitch(Random.Range(0.9f, 1.1f), 0.5f);
            source.PlayOneShot(clip);
        }
    }

    public static void PlayOneshot(AudioClip clip, AudioSource source, float volume)
    {
        if (clip && source)
        {
            source.DOPitch(Random.Range(0.9f, 1.1f), 0.5f);
            source.PlayOneShot(clip, volume);
        }
    }

    public static void PlayOneshot(AudioClip[] clips, AudioSource source)
    {
        if (clips.Length > 0 && source)
        {
            source.DOPitch(Random.Range(0.9f, 1.1f), 0.5f);
            source.PlayOneShot(clips[Random.Range(0, clips.Length)]);
        }
    }

    public static void PlayOneshot(AudioClip[] clips, AudioSource source, float volume)
    {
        if (clips.Length > 0 && source)
        {
            source.DOPitch(Random.Range(0.9f, 1.1f), 0.5f);
            source.PlayOneShot(clips[Random.Range(0, clips.Length)], volume);
        }
    }

    public static void Play(AudioClip[] clips, AudioSource source)
    {
        if (clips.Length > 0 && source)
        {
            source.DOPitch(Random.Range(0.9f, 1.1f), 0.5f);
            source.clip = clips[Random.Range(0, clips.Length)];
            source.Play();
        }
    }
}
