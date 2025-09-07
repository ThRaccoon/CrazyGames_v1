using UnityEngine;
using UnityEngine.UIElements;

public class InGameUIController : IControllerUI, IPlayerBoundUI
{
    private VisualElement _root;

    private Button _optionsButton;

    private VisualElement _hpBar;
    private VisualElement _xpBar;

    private Player _player;

    public void Init(VisualElement root)
    {
        _root = root;

        _optionsButton = root.Q<Button>("OptionsButton");

        _hpBar = root.Q<VisualElement>("HpBar");
        _xpBar = root.Q<VisualElement>("XpBar");
    }

    public void OnActivate()
    {
        _optionsButton.RegisterCallback<ClickEvent>(OnMenu);

        _player.OnHealthChanged += UpdateHpBar;

        if (_hpBar != null)
            _hpBar.style.width = Length.Percent(100f);

        if (_xpBar != null)
            _xpBar.style.width = Length.Percent(0f);
    }

    public void OnDeactivate()
    {
        _optionsButton.UnregisterCallback<ClickEvent>(OnMenu);

        _player.OnHealthChanged -= UpdateHpBar;
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

    public void UpdateHpBar(float newValue)
    {
        _hpBar.style.width = Length.Percent(newValue * 100f);
    }

    public void UpdateXpBar(float newValue)
    {
        _xpBar.style.width = Length.Percent(newValue * 100f);
    }
}