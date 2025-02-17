using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource MusicSource;

    public AudioClip music;
    public AudioClip SFX;

    private void Start()
    {
        MusicSource.clip = music;
        MusicSource.Play();
    }

    private void Update()
    {
        MusicSource.volume = PlayerPrefs.GetFloat("musicVolume");
    }
}
