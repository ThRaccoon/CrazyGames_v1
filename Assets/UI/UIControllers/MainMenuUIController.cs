using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUIController : IControllerUI
{
    private VisualElement _root;

    private Button _playButton;
    private Button _upgradesButton;
    private Button _quitButton;
    private Button _optionsButton;

    public void Init(VisualElement root)
    {
        _root = root;

        _playButton = root.Q<Button>("PlayButton");
        _upgradesButton = root.Q<Button>("UpgradesButton");
        _quitButton = root.Q<Button>("QuitButton");
        _optionsButton = root.Q<Button>("OptionsButton");
    }

    public void OnActivate()
    {
        _playButton?.RegisterCallback<ClickEvent>(OnPlay);
        _upgradesButton?.RegisterCallback<ClickEvent>(OnUpgrades);
        _quitButton?.RegisterCallback<ClickEvent>(OnQuit);
        _optionsButton?.RegisterCallback<ClickEvent>(OnOptions);
    }

    public void OnDeactivate()
    {
        _playButton?.UnregisterCallback<ClickEvent>(OnPlay);
        _upgradesButton?.UnregisterCallback<ClickEvent>(OnUpgrades);
        _quitButton?.UnregisterCallback<ClickEvent>(OnQuit);
        _optionsButton?.UnregisterCallback<ClickEvent>(OnOptions);
    }


    public void OnPlay(ClickEvent evt)
    {
        AudioManager.Instance.PlayClickSound();

        GameManager.Instance.SetState(EGameState.InGame);
    }

    public void OnUpgrades(ClickEvent evt)
    {
        AudioManager.Instance.PlayClickSound();

        UIManager.Instance.ShowOverlay("Upgrades");
    }

    public void OnQuit(ClickEvent evt)
    {
        AudioManager.Instance.PlayClickSound();

        Application.Quit();
    }

    public void OnOptions(ClickEvent evt)
    {
        AudioManager.Instance.PlayClickSound();

        UIManager.Instance.ShowOverlay("Options");
    }
}