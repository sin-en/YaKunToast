using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Background Music")]
    public AudioClip backgroundMusic;
    [Header("Sound Effects")]
    public AudioClip collectChime;

    private AudioSource musicAudioSource;
    private AudioSource sfxAudioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        musicAudioSource = gameObject.AddComponent<AudioSource>();
        sfxAudioSource = gameObject.AddComponent<AudioSource>();
    }
    private void Start()
    {
        PlayBackgroundMusic();
    }
    private void PlayBackgroundMusic()
    {
        musicAudioSource.clip = backgroundMusic;
        musicAudioSource.loop = true;
        musicAudioSource.Play();
    }
    public void PlayCollectChime()
    {
        sfxAudioSource.PlayOneShot(collectChime);
    }
}