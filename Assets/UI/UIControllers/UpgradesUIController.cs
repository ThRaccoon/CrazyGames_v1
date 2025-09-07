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

    public void Init(VisualElement root)
    {
        _root = root;

        _leftCard = root.Q<VisualElement>("LeftCard");
        _cards.Add(_leftCard);

        _middleCard = root.Q<VisualElement>("MiddleCard");
        _cards.Add(_middleCard);

        _rightCard = root.Q<VisualElement>("RightCard");
        _cards.Add(_rightCard);

        foreach (VisualElement card in _cards)
        {
            SetupCardsScaleTransition(card, "scale", 0.2f, EasingMode.Ease);
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


    public void OnLeftCard(ClickEvent evt)
    {
        AudioManager.Instance.PlayClickSound();

        Debug.Log("Left Card");
    }

    public void OnMiddleCard(ClickEvent evt)
    {
        AudioManager.Instance.PlayClickSound();

        Debug.Log("Middle Card");
    }

    public void OnRightCard(ClickEvent evt)
    {
        AudioManager.Instance.PlayClickSound();

        Debug.Log("Right Card");
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


    private void SetupCardsScaleTransition(VisualElement element, string property, float duration, EasingMode easingMode)
    {
        element.style.transitionProperty = new List<StylePropertyName> { new StylePropertyName(property) };
        element.style.transitionDuration = new List<TimeValue> { new TimeValue(duration, TimeUnit.Second) };
        element.style.transitionTimingFunction = new List<EasingFunction> { new EasingFunction(easingMode) };
    }

    private async Task AnimateCardsRotation()
    {
        if (!_canRotateCards) return;
        _canRotateCards = false;

        foreach (var card in _cards)
        {
            if (card != null)
            {
                card.style.scale = new Scale(new Vector2(1.1f, 1.1f));
            }
        }

        await Task.Delay(200);

        foreach (var card in _cards)
        {
            if (card != null)
            {
                card.style.scale = StyleKeyword.Null;
            }
        }

        _canRotateCards = true;
    }
}