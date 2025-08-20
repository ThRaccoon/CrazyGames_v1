using UnityEngine.UIElements;

public class InGameUIController : IUIController, IPlayerBoundUI
{
    private VisualElement _root;

    private Button _optionsButton;

    private VisualElement _healthBar;
    private VisualElement _xpBar;

    private Player _player;

    public void Init(VisualElement root)
    {
        _root = root;

        _optionsButton = root.Q<Button>("OptionsButton");

        _healthBar = root.Q<VisualElement>("HealthBar");
        _xpBar = root.Q<VisualElement>("XpBar");
    }

    public void OnActivate()
    {
        _optionsButton.RegisterCallback<ClickEvent>(OnMenu);

        _player.OnHealthChanged += UpdateHealthBar;
    }

    public void OnDeactivate()
    {
        _optionsButton.UnregisterCallback<ClickEvent>(OnMenu);
    
        _player.OnHealthChanged -= UpdateHealthBar;
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

    public void UpdateHealthBar(float newValue)
    {
        _healthBar.style.width = Length.Percent(newValue * 100f);
    }

    public void UpdateXpBar(float newValue)
    {
        _xpBar.style.width = Length.Percent(newValue * 100f);
    }
}