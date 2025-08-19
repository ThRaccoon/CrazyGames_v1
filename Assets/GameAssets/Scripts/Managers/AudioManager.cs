using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    // ====================================================================================================
    [Header("Components")]
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioSource _SFXSource;

    // ====================================================================================================

    // ====================================================================================================
    [Space(15)]
    [Header("Click Sound")]
    [SerializeField] private AudioClip _clickSound;
    [SerializeField] private float _clickSoundVolume;
    // ====================================================================================================

    private bool _isSFXEnabled = true;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        // === Singleton ===

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // ==============================

    }

    private void Start()
    {
        bool musicOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        ToggleMusic(musicOn);

        bool SFXOn = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;
        ToggleSFX(SFXOn);
    }

    public void PlaySFXClip(AudioClip clip, float volume, Vector3 spawnPoint)
    {
        if (!_isSFXEnabled) return;

        if (_SFXSource == null || clip == null) return;

        volume = Mathf.Clamp(volume, 0f, 1f);

        AudioSource tempAudioSource = Instantiate(_SFXSource, spawnPoint, Quaternion.identity);

        tempAudioSource.clip = clip;
        tempAudioSource.volume = volume;

        tempAudioSource.Play();

        Destroy(tempAudioSource.gameObject, tempAudioSource.clip.length);
    }

    public void ToggleMusic(bool flag)
    {
        if (flag)
        {
            _audioMixer.SetFloat("Music", 0f);
        }
        else
        {
            _audioMixer.SetFloat("Music", -80f);
        }

        PlayerPrefs.SetInt("MusicEnabled", flag ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleSFX(bool flag)
    {
        if (flag)
        {
            _audioMixer.SetFloat("SoundFX", 0f);
            _isSFXEnabled = true;
        }
        else
        {
            _audioMixer.SetFloat("SoundFX", -80f);
            _isSFXEnabled = false;
        }

        PlayerPrefs.SetInt("SFXEnabled", flag ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void PlayClickSound()
    {
        PlaySFXClip(_clickSound, _clickSoundVolume, Vector3.zero);
    }
}