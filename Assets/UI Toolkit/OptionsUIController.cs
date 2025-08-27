using UnityEngine;
using UnityEngine.UIElements;

public class OptionsUIController : IUIController
{
    private VisualElement _root;

    private VisualElement _buttonsContainer;

    private Button _closeButton;
    private Button _resumeButton;
    private Button _menuButton;

    private Toggle _musicToggle;
    private Toggle _soundToggle;

    public void Init(VisualElement root)
    {
        _root = root;

        _buttonsContainer = root.Q<VisualElement>("ButtonsContainer");

        _closeButton = root.Q<Button>("CloseButton");
        _resumeButton = root.Q<Button>("ResumeButton");
        _menuButton = root.Q<Button>("MenuButton");

        _musicToggle = root.Q<Toggle>("MusicToggle");
        _soundToggle = root.Q<Toggle>("SoundToggle");
    }

    public void OnActivate()
    {
        GameManager.Instance.Pause();

        _closeButton?.RegisterCallback<ClickEvent>(OnClose);

        _resumeButton?.RegisterCallback<ClickEvent>(OnResume);
        _menuButton?.RegisterCallback<ClickEvent>(OnMenu);

        _musicToggle?.RegisterCallback<ChangeEvent<bool>>(OnMusic);
        _soundToggle?.RegisterCallback<ChangeEvent<bool>>(OnSFX);

        if (_musicToggle != null)
            _musicToggle.value = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;

        if (_soundToggle != null)
            _soundToggle.value = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;

        UpdateVisibility();
    }

    public void OnDeactivate()
    {
        GameManager.Instance.Unpause();
        
        _closeButton?.UnregisterCallback<ClickEvent>(OnClose);
        _resumeButton?.UnregisterCallback<ClickEvent>(OnResume);
        _menuButton?.UnregisterCallback<ClickEvent>(OnMenu);

        _musicToggle?.UnregisterCallback<ChangeEvent<bool>>(OnMusic);
        _soundToggle?.UnregisterCallback<ChangeEvent<bool>>(OnSFX);
    }


    public void OnClose(ClickEvent evt)
    {
        AudioManager.Instance.PlayClickSound();

        UIManager.Instance.HideOverlayUI();
    }

    public void OnResume(ClickEvent evt)
    {
        AudioManager.Instance.PlayClickSound();

        UIManager.Instance.HideOverlayUI();
    }

    public void OnMenu(ClickEvent evt)
    {
        AudioManager.Instance.PlayClickSound();

        GameManager.Instance.SetState(EGameState.MainMenu);
    }

    public void OnMusic(ChangeEvent<bool> evt)
    {
        AudioManager.Instance.PlayClickSound();

        AudioManager.Instance.ToggleMusic(evt.newValue);
    }

    public void OnSFX(ChangeEvent<bool> evt)
    {
        AudioManager.Instance.PlayClickSound();

        AudioManager.Instance.ToggleSFX(evt.newValue);
    }

    private void UpdateVisibility()
    {
        if (GameManager.Instance.CurrentState != EGameState.InGame) 
        {
            _buttonsContainer.style.height = Length.Percent(50f);
            
            _resumeButton.style.display = DisplayStyle.None;
            _menuButton.style.display = DisplayStyle.None;
        }
    }
}