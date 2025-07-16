using UnityEngine;

public class GameManager : MonoBehaviour
{

    [HideInInspector] public static GameManager _SGameManager;
    [SerializeField] AudioSource _audioSource;
    private bool _isPaused = false;
    private bool _canToggle = true;

    private void Awake()
    {
        _SGameManager = this;
    }

    public void PlayButtonSound()
    {
        if( _audioSource != null ) 
        {
            _audioSource.Play();
        }
    }

    public void PauseGame()
    {
        _canToggle = false;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        _canToggle = true;
        Time.timeScale = 1f;
    }

    public void PauseToggle()
    {
        if (_canToggle == false)
            return;

        PlayButtonSound();

        if ( _isPaused )
        {
            Time.timeScale = 1f;
        }
        else
        {
            Time.timeScale = 0f;
        }

        _isPaused = !_isPaused;

    }


}
