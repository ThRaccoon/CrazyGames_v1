using UnityEngine;
using UnityEngine.SceneManagement;

public enum EGameState { None, MainMenu, InGame }

public class GameManager : MonoBehaviour
{
    private Player _player;

    public EGameState CurrentState { get; private set; }

    public static GameManager Instance { get; private set; }

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
        SetState(EGameState.MainMenu);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MainMenuScene":
                {
                    UIManager.Instance.ClearScreenUI();
                    UIManager.Instance.HideOverlayUI();
                    UIManager.Instance.UpdateScreenUI("MainMenu");
                }
                break;
            case "InGameScene":
                {
                    _player = FindFirstObjectByType<Player>();

                    UIManager.Instance.BindPlayer(_player);

                    UIManager.Instance.ClearScreenUI();
                    UIManager.Instance.HideOverlayUI();
                    UIManager.Instance.UpdateScreenUI("InGame");
                }
                break;
        }
    }

    public void SetState(EGameState newState)
    {
        CurrentState = newState;

        switch (newState)
        {
            case EGameState.MainMenu:
                {
                    SceneManager.LoadScene("MainMenuScene");
                }
                break;
            case EGameState.InGame:
                {
                    SceneManager.LoadScene("InGameScene");
                }
                break;
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
    }

    public void Unpause()
    {
        Time.timeScale = 1f;
    }
}