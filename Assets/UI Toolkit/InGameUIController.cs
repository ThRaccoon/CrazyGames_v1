using UnityEngine;
using UnityEngine.UIElements;

public class InGameUIController : IUIController, IPlayerBoundUI
{
    private VisualElement _root;

    private Button _optionsButton;

    private VisualElement _healthProgress;
    private VisualElement _xpProgress;

    private Player _player;

    public void Init(VisualElement root)
    {
        _root = root;

        _optionsButton = root.Q<Button>("OptionsButton");

        _healthProgress = root.Q<VisualElement>("HealthProgress");
        _xpProgress = root.Q<VisualElement>("XpProgress");
    }

    public void OnActivate()
    {
        _optionsButton.RegisterCallback<ClickEvent>(OnMenu);

        _player.OnHealthChanged += SetHealthProgress;
    }

    public void OnDeactivate()
    {
        _optionsButton.UnregisterCallback<ClickEvent>(OnMenu);
    
        _player.OnHealthChanged -= SetHealthProgress;
    }

    public void BindPlayer(Player player) 
    {
        _player = player;
    }

    public void OnMenu(ClickEvent evt)
    {
        AudioManager.Instance.PlayClickSound();

        UIManager.Instance.ShowOverlay("Options");
    }

    public void SetHealthProgress(float newValue)
    {
        _healthProgress.style.width = Length.Percent(newValue * 100f);
    }

    public void SetXpProgress(float newValue)
    {
        _xpProgress.style.width = Length.Percent(newValue * 100f);
    }
}