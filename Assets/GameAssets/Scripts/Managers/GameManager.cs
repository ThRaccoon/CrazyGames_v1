using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GameManager : MonoBehaviour
{
    [SerializeField] private AudioClip _clickSound;
    [SerializeField, Range(0.0f, 1.0f)] private float _clickSoundVolume;

    private bool _isPaused;
    private bool _canToggle = true;

    public static GameManager _SGameManager;

    private void Awake()
    {
        _SGameManager = this;
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
        if (_canToggle == false) return;

        AudioManager.SAudioManager.PlayButtonSound();

        if (_isPaused)
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
