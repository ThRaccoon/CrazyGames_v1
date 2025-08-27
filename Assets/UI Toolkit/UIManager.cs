using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class UIEntry
{
    [field: SerializeField] public string Key { get; private set; }
    [field: SerializeField] public VisualTreeAsset Uxml { get; private set; }
    [field: SerializeField] public string UIController { get; private set; }
}

public class UIManager : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private List<UIEntry> _uiEntries;

    private Dictionary<string, UIEntry> _uiEntriesLookup;

    private VisualElement _currentScreen;
    public IUIController _currentScreenController { get; private set; }

    private VisualElement _currentOverlay;
    private IUIController _currentOverlayController;

    private Player _player;

    public static UIManager Instance { get; private set; }

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

        _uiEntriesLookup = new Dictionary<string, UIEntry>();
        foreach (var entry in _uiEntries)
        {
            if (!_uiEntriesLookup.ContainsKey(entry.Key))
            {
                _uiEntriesLookup.Add(entry.Key, entry);
            }
            else
            {
                Debug.LogWarning($"Duplicate UIEntry key: {entry.Key}");
            }
        }
    }

    public void UpdateScreenUI(string key)
    {
        if (_uiDocument == null) return;

        if (!_uiEntriesLookup.ContainsKey(key)) return;

        UIEntry entry = _uiEntriesLookup[key];
        _currentScreen = entry.Uxml.CloneTree();
        
        _currentScreen.style.flexGrow = 1;

        _uiDocument.rootVisualElement.Add(_currentScreen);

        Type type = Type.GetType(entry.UIController);
        _currentScreenController = (IUIController)Activator.CreateInstance(type);

        if (_currentScreenController is IPlayerBoundUI playerBound)
        {
            playerBound.BindPlayer(_player);
        }

        _currentScreenController.Init(_currentScreen);

        _currentScreenController.OnActivate();
    }

    public void ClearScreenUI()
    {
        _currentScreenController?.OnDeactivate();
        _currentScreenController = null;

        _currentScreen?.RemoveFromHierarchy();
        _currentScreen = null;

        _uiDocument?.rootVisualElement.Clear();
    }

    public void ShowOverlay(string key)
    {
        if (_currentOverlay != null) return;

        if (_uiDocument == null) return;

        if (!_uiEntriesLookup.ContainsKey(key)) return;

        UIEntry entry = _uiEntriesLookup[key];
        _currentOverlay = entry.Uxml.CloneTree();

        _currentOverlay.style.position = Position.Absolute;
        _currentOverlay.style.width = Length.Percent(100);
        _currentOverlay.style.height = Length.Percent(100);

        _uiDocument.rootVisualElement.Add(_currentOverlay);

        Type type = Type.GetType(entry.UIController);
        _currentOverlayController = (IUIController)Activator.CreateInstance(type);
        
        _currentOverlayController.Init(_uiDocument.rootVisualElement);

        _currentOverlayController.OnActivate();
    }

    public void HideOverlayUI()
    {
        _currentOverlayController?.OnDeactivate();
        _currentOverlayController = null;

        _currentOverlay?.RemoveFromHierarchy();
        _currentOverlay = null;
    }

    public void BindPlayer(Player player)
    {
        _player = player;
    }
}