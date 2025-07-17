using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioSource _soundFX;
    [SerializeField] private AudioSource _buttonClickSound;

    [SerializeField] private AudioClip _clickSound;
    [SerializeField] private float _clickSoundVolume;

    private bool _isSoundFXEnabled = true;
    private bool _isMusicEnabled = true;

    // Singleton
    public static AudioManager SAudioManager;

    private void Awake()
    {
        // Singleton
        if (SAudioManager == null)
        {
            SAudioManager = this;
        }
    }

    public void PlaySoundFXClip(AudioClip clip, float volume, Vector3 spawnPoint)
    {
        if (!_isSoundFXEnabled) return;

        if (_soundFX == null || clip == null) return;

        volume = Mathf.Clamp(volume, 0f, 1f);

        AudioSource tempAudioSource = Instantiate(_soundFX, spawnPoint, Quaternion.identity);

        tempAudioSource.clip = clip;
        tempAudioSource.volume = volume;

        tempAudioSource.Play();

        Destroy(tempAudioSource.gameObject, tempAudioSource.clip.length);
    }

    // --- Interface ---
    public void ToggleSoundFX()
    {
        PlayButtonSound();

        _isSoundFXEnabled = !_isSoundFXEnabled;

        if (_isSoundFXEnabled)
        {
            _audioMixer.SetFloat("SoundFX", -80f);
        }
        else
        {
            _audioMixer.SetFloat("SoundFX", 0f);
        }
    }

    public void ToggleMusic()
    {
        PlayButtonSound();

        _isMusicEnabled = !_isMusicEnabled;

        if (!_isMusicEnabled)
        {
            _audioMixer.SetFloat("Music", -80f);
        }
        else
        {
            _audioMixer.SetFloat("Music", 0f);
        }
    }

    public void PlayButtonSound()
    {
        _buttonClickSound.Play();
    }
}