using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUIController : IUIController
{
    private VisualElement _root;

    private Button _playButton;
    private Button _cardsButton;
    private Button _quitButton;
    private Button _optionsButton;

    public void Init(VisualElement root)
    {
        _root = root;

        _playButton = root.Q<Button>("PlayButton");
        _cardsButton = root.Q<Button>("CardsButton");
        _quitButton = root.Q<Button>("QuitButton");
        _optionsButton = root.Q<Button>("OptionsButton");
    }

    public void OnActivate()
    {
        _playButton?.RegisterCallback<ClickEvent>(OnPlay);
        _cardsButton?.RegisterCallback<ClickEvent>(OnCards);
        _quitButton?.RegisterCallback<ClickEvent>(OnQuit);
        _optionsButton?.RegisterCallback<ClickEvent>(OnOptions);
    }

    public void OnDeactivate()
    {
        _playButton?.UnregisterCallback<ClickEvent>(OnPlay);
        _cardsButton?.UnregisterCallback<ClickEvent>(OnCards);
        _quitButton?.UnregisterCallback<ClickEvent>(OnQuit);
        _optionsButton?.UnregisterCallback<ClickEvent>(OnOptions);
    }


    public void OnPlay(ClickEvent evt)
    {
        AudioManager.Instance.PlayClickSound();

        GameManager.Instance.SetState(EGameState.InGame);
    }

    public void OnCards(ClickEvent evt)
    {
        AudioManager.Instance.PlayClickSound();

        Debug.Log("Cards");
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