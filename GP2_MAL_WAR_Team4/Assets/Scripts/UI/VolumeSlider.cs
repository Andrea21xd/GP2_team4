using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider MasterSlider;
    [SerializeField] private Slider MusicSlider;

    private void Awake()
    {
        LoadVolume();
    }

    void Start()
    {
        
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMusicVolume();
            SetMasterVolume();
        }

    }
        public void SetMusicVolume()
        {
            float volume = MusicSlider.value;
            myMixer.SetFloat("music", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("musicVolume", volume);
            PlayerPrefs.Save();
        }

        public void SetMasterVolume()
        {
            float volume = MasterSlider.value;
            myMixer.SetFloat("master", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("masterVolume", volume);
            PlayerPrefs.Save();
        }

        public void LoadVolume()
        {
            MusicSlider.value = PlayerPrefs.GetFloat("musicVolume");
            MasterSlider.value = PlayerPrefs.GetFloat("masterVolume");
            PlayerPrefs.Save();

            SetMusicVolume();
            SetMasterVolume();
        }
    
}
