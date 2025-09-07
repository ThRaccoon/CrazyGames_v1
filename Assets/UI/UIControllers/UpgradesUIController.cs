using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class UpgradesUIController : IControllerUI
{
    private VisualElement _root;

    private VisualElement _leftCard;
    private VisualElement _middleCard;
    private VisualElement _rightCard;
    private List<VisualElement> _cards = new List<VisualElement>();

    private Button _closeButton;
    private Button _prevButton;
    private Button _nextButton;

    private bool _canRotateCards = true;
    private bool _canClickCards = true;
    private float _cardTransitionDuration = 0.2f;
    private string[] properties;
    private Scale _scaleUp = new Scale(new Vector2(1.1f, 1.1f));
    private Color _originalCardColor = new Color(245f / 255f, 230f / 255f, 205f / 255f);
    private Color _onClickCardColor = new Color(220f / 255f, 200f / 255f, 180f / 255f);

    public void Init(VisualElement root)
    {
        _root = root;

        _leftCard = root.Q<VisualElement>("LeftCard");
        _cards.Add(_leftCard);

        _middleCard = root.Q<VisualElement>("MiddleCard");
        _cards.Add(_middleCard);

        _rightCard = root.Q<VisualElement>("RightCard");
        _cards.Add(_rightCard);

        properties = new string[] { "scale", "background-color" };

        foreach (VisualElement card in _cards)
        {
            SetupCardsScaleTransition(card, properties, _cardTransitionDuration, EasingMode.Ease);
        }

        _closeButton = root.Q<Button>("CloseButton");
        _prevButton = root.Q<Button>("PrevButton");
        _nextButton = root.Q<Button>("NextButton");
    }

    public void OnActivate()
    {
        _leftCard?.RegisterCallback<ClickEvent>(OnLeftCard);
        _middleCard?.RegisterCallback<ClickEvent>(OnMiddleCard);
        _rightCard?.RegisterCallback<ClickEvent>(OnRightCard);

        _closeButton?.RegisterCallback<ClickEvent>(OnClose);
        _prevButton?.RegisterCallback<ClickEvent>(OnPrev);
        _nextButton?.RegisterCallback<ClickEvent>(OnNext);
    }

    public void OnDeactivate()
    {
        _leftCard?.UnregisterCallback<ClickEvent>(OnLeftCard);
        _middleCard?.UnregisterCallback<ClickEvent>(OnMiddleCard);
        _rightCard?.UnregisterCallback<ClickEvent>(OnRightCard);

        _closeButton?.UnregisterCallback<ClickEvent>(OnClose);
        _prevButton?.UnregisterCallback<ClickEvent>(OnPrev);
        _nextButton?.UnregisterCallback<ClickEvent>(OnNext);
    }


    public async void OnLeftCard(ClickEvent evt)
    {
        if (!_canClickCards) return;

        AudioManager.Instance.PlayClickSound();

        Debug.Log("Left Card");

        await ChangeCardColorOnClick(_leftCard);
    }

    public async void OnMiddleCard(ClickEvent evt)
    {
        if (!_canClickCards) return;

        AudioManager.Instance.PlayClickSound();

        Debug.Log("Middle Card");

        await ChangeCardColorOnClick(_middleCard);
    }

    public async void OnRightCard(ClickEvent evt)
    {
        if (!_canClickCards) return;

        AudioManager.Instance.PlayClickSound();

        Debug.Log("Right Card");

        await ChangeCardColorOnClick(_rightCard);
    }


    public void OnClose(ClickEvent evt)
    {
        AudioManager.Instance.PlayClickSound();

        UIManager.Instance.HideOverlayUI();
    }

    public async void OnPrev(ClickEvent evt)
    {
        if (!_canRotateCards) return;

        AudioManager.Instance.PlayClickSound();

        await AnimateCardsRotation();
    }

    public async void OnNext(ClickEvent evt)
    {
        if (!_canRotateCards) return;

        AudioManager.Instance.PlayClickSound();

        await AnimateCardsRotation();
    }


    private void SetupCardsScaleTransition(VisualElement element, string[] properties, float duration, EasingMode easingMode)
    {
        element.style.transitionProperty = new List<StylePropertyName> { new StylePropertyName(properties[0]), new StylePropertyName(properties[1]) };
        element.style.transitionDuration = new List<TimeValue> { new TimeValue(duration, TimeUnit.Second), new TimeValue(duration, TimeUnit.Second) };
        element.style.transitionTimingFunction = new List<EasingFunction> { new EasingFunction(easingMode), new EasingFunction(easingMode) };
    }

    private async Task AnimateCardsRotation()
    {
        if (!_canRotateCards) return;
        _canRotateCards = false;

        foreach (var card in _cards)
        {
            if (card != null)
            {
                card.style.scale = _scaleUp;
            }
        }

        await Task.Delay((int)(_cardTransitionDuration * 1000));

        foreach (var card in _cards)
        {
            if (card != null)
            {
                card.style.scale = StyleKeyword.Null;
            }
        }

        _canRotateCards = true;
    }

    private async Task ChangeCardColorOnClick(VisualElement element)
    {
        if (!_canClickCards) return;
        _canClickCards = false;

        element.style.backgroundColor = _onClickCardColor;

        await Task.Delay((int)(_cardTransitionDuration * 1000));

        element.style.backgroundColor = _originalCardColor;

        _canClickCards = true;
    }
}