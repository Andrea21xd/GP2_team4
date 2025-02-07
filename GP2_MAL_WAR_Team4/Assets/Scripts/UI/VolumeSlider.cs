using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider MasterSlider;
    [SerializeField] private Slider MusicSlider;

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


         void SetMusicVolume()
        {
            float volume = MusicSlider.value;
            myMixer.SetFloat("music", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("musicVolume", volume);
        }

        void SetMasterVolume()
        {
            float volume = MasterSlider.value;
            myMixer.SetFloat("master", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("masterVolume", volume);
        }

        void LoadVolume()
        {
            MusicSlider.value = PlayerPrefs.GetFloat("musicVolume");
            MasterSlider.value = PlayerPrefs.GetFloat("masterVolume");

            SetMusicVolume();
            SetMasterVolume();
        }
    }
}
