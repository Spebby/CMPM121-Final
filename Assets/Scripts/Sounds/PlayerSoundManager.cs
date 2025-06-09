using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

using CMPM.Core;
using CMPM.UI;

public enum SoundTypePlayer
{
    WALK,
    HURT,
    DEATH,
    ATTACK,
}

[RequireComponent(typeof(AudioSource))]

public class PlayerSoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] playerSoundsList;
    private static PlayerSoundManager instance;
    private AudioSource audioSource;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    public static void PlaySound(SoundTypePlayer playerSound, float volume = 1)
    {
        instance.audioSource.PlayOneShot(instance.playerSoundsList[(int)playerSound], volume);
    }
    public static AudioClip GetPlayerClip(SoundTypePlayer playerSound)
    {
        return instance.playerSoundsList[(int)playerSound];
    }

    public static void StartLooping(SoundTypePlayer playerSound, float volume = 1f)
    {
        if (instance.audioSource.isPlaying && instance.audioSource.loop && instance.audioSource.clip == instance.playerSoundsList[(int)playerSound])
        {
            return;
        }
        instance.audioSource.clip = instance.playerSoundsList[(int)playerSound];
        instance.audioSource.loop = true;
        instance.audioSource.volume = volume;
        instance.audioSource.Play();
    }
    public static void StopLooping(){
        if (instance.audioSource.isPlaying && instance.audioSource.loop)
        {
            instance.audioSource.Stop();
            instance.audioSource.loop = false;
            instance.audioSource.clip = null;
        }
    }
}