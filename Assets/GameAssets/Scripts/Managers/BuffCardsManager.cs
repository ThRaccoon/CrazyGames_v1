using System;
using TMPro;
using UnityEngine;

[System.Serializable]
public class BuffCard
{
    public EStatsType statsType;
    public Sprite icon;
    public Vector2 valueRange;
    [TextArea] public string description;
}

[System.Serializable]
public class RandomBuffCard
{
    public EStatsType statsType;
    public UnityEngine.UI.Image iconHolder;
    public TextMeshProUGUI valueHolder;
    public TextMeshProUGUI descriptionHolder;

    [HideInInspector] public float randomizedBuffValue;
}

public class BuffCardsManager : MonoBehaviour
{
    // ====================================================================================================
    [Header("Components")]
    [SerializeField] Player _player;
    [SerializeField] GameObject _canvas;
    [Space(15)]
    [SerializeField] BuffCard[] _buffCards;
    [SerializeField] RandomBuffCard[] _randomBuffCards;
    // ====================================================================================================

    public static BuffCardsManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RollBuffs()
    {
        GameManager.Instance.Pause();

        GetRandomUniqueBuffs();

        _canvas.SetActive(true);
    }

    public void OnBuffCardSelect(int index)
    {
        GameManager.Instance.Unpause();

        _player.ApplyBuff(_randomBuffCards[index].statsType, _randomBuffCards[index].randomizedBuffValue);

        _canvas.SetActive(false);
    }

    public void GetRandomUniqueBuffs()
    {
        if (_buffCards.Length < _randomBuffCards.Length) return;

        ShuffleBuffCards();

        for (int i = 0; i < _randomBuffCards.Length; i++)
        {
            if (_randomBuffCards[i].iconHolder != null)
            {
                _randomBuffCards[i].iconHolder.sprite = _buffCards[i].icon;
            }

            if (_randomBuffCards[i].descriptionHolder != null)
            {
                _randomBuffCards[i].descriptionHolder.text = _buffCards[i].description;
            }
            _randomBuffCards[i].randomizedBuffValue = (float)Math.Round(UnityEngine.Random.Range(_buffCards[i].valueRange.x, _buffCards[i].valueRange.y), 1);

            if (_randomBuffCards[i].valueHolder)
            {
                _randomBuffCards[i].valueHolder.text = "+ " + _randomBuffCards[i].randomizedBuffValue + "%";
            }
            _randomBuffCards[i].statsType = _buffCards[i].statsType;
        }
    }

    private void ShuffleBuffCards()
    {
        System.Random random = new System.Random();
        int remaining = _buffCards.Length;

        while (remaining > 1)
        {
            remaining--;

            int randomIndex = random.Next(remaining + 1);
            (_buffCards[randomIndex], _buffCards[remaining]) = (_buffCards[remaining], _buffCards[randomIndex]);
        }
    }
}