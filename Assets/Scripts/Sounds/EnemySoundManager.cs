using UnityEngine;


public enum SoundTypeEnemies
{
    ZOMBIE,
    SKELETON,
    WARLOCK,
    DRYAD,
    GOAT
}

[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]

public class EnemiesSoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] enemySoundsList;
    private static EnemiesSoundManager instance;
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
    public static void PlaySound(SoundTypeEnemies enemySound, float volume = 1)
    {
        instance.audioSource.PlayOneShot(instance.enemySoundsList[(int)enemySound], volume);
    }
    public static AudioClip GetEnemyClip(SoundTypeEnemies enemySound)
    {
        return instance.enemySoundsList[(int)enemySound];
    }
}