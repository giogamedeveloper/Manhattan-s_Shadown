using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioController : MonoBehaviour
{
    [System.Serializable]
    public struct AudioData
    {
        public AudioClip clip;
        public string name;

        [Range(0.1f, 1f)]
        public float volume;
    }

    public AudioMixer audioMixer;
    [SerializeField] AudioClip _menuMusic;
    [SerializeField] AudioClip _gameMusic;

    public AudioData[] sounds;
    public AudioSource soundsAudioSource;

    [SerializeField]
    private AudioSource musicAudioSource;

    private static AudioController _instance;
    public static AudioController Instance => _instance;

    [SerializeField]
    private float volume;

    [SerializeField]
    private float generalVolume;

    private float musicVolume;
    private float sfxVolume;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            generalVolume = PlayerPrefs.GetFloat("GeneralMusic", 1f);
            musicVolume = PlayerPrefs.GetFloat("Music", 1f);
            sfxVolume = PlayerPrefs.GetFloat("SFX", 1f);

            SetAudioMixerVolumes();
        }
        else
        {
            Destroy(gameObject);
        }
        SetMusicForScene(SceneManager.GetActiveScene().name);
    }

    private void SetAudioMixerVolumes()
    {
        float gVolume = Mathf.Max(generalVolume, 0.0001f);
        float mVolume = Mathf.Max(musicVolume, 0.0001f);
        float sVolume = Mathf.Max(sfxVolume, 0.0001f);

        audioMixer.SetFloat("GeneralMusic", Mathf.Log10(gVolume) * 20);
        audioMixer.SetFloat("Music", Mathf.Log10(mVolume) * 20);
        audioMixer.SetFloat("SFX", Mathf.Log10(sVolume) * 20);
    }

    public void PlaySound(string name)
    {
        if (TryGetAudioDataWithName(out AudioData audioData, name))
        {
            soundsAudioSource.PlayOneShot(audioData.clip, audioData.volume);
        }
        else
        {
            Debug.LogWarning("No audio clip found with name: " + name);
        }
    }

    /// <summary>
    /// Devuelve el AudioData cuyo nombre coincida con el indicado 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private bool TryGetAudioDataWithName(out AudioData audioData, string name)
    {
        audioData = new AudioData();
        foreach (AudioData data in sounds)
        {
            if (data.name == name)
            {
                audioData = data;
                return true;
            }
        }
        return false;
    }


    public void SetMusicForScene(string sceneName)
    {
        if (sceneName == "MainMenu")
        {
            musicAudioSource.clip = _menuMusic;
        }
        else if (sceneName == "Game")
        {
            musicAudioSource.clip = _gameMusic;
        }

        if (!musicAudioSource.isPlaying)
        {
            musicAudioSource.Play();
        }
    }

    public void SetMusicVolume(float _volume)
    {
        volume = Mathf.Log10(_volume) * 20;
        PlayerPrefs.SetFloat("Music", _volume);
        audioMixer.SetFloat("Music", volume);
        PlayerPrefs.Save();
    }

    public void SetGeneralVolume(float _volume)
    {
        volume = Mathf.Log10(_volume) * 20;
        PlayerPrefs.SetFloat("GeneralMusic", _volume);
        audioMixer.SetFloat("GeneralMusic", volume);
        PlayerPrefs.Save();
    }

    public void SetEffectVolume(float _volume)
    {
        volume = Mathf.Log10(_volume) * 20;
        PlayerPrefs.SetFloat("SFX", _volume);
        audioMixer.SetFloat("SFX", volume);
        PlayerPrefs.Save();
    }
}
