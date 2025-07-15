using System;
using TMPro;
using UnityEngine;

[System.Serializable]
public class BuffCard
{
    public EStatsType statsType;
    public float minValue;
    public float maxValue;
    public Sprite icon;
    [TextArea] public string description;
}

[System.Serializable]
public class RandomBuffCard
{
    public UnityEngine.UI.Image iconHolder;
    public TextMeshProUGUI descriptionHolder;
    public float value;
    public TextMeshProUGUI valueHolder;
    public EStatsType statsType;
}

public class BuffCardManager : MonoBehaviour
{
    [HideInInspector] public static BuffCardManager _SBuffCardManagerScript;
    [SerializeField] BuffCard[] _buffCards;
    [SerializeField] RandomBuffCard[] _randomBuffCards;
    [SerializeField] GameObject _buffParent;

    bool _canRoll;

    void Awake()
    {
        _SBuffCardManagerScript = this;

        if (_buffParent)
        {
            _buffParent.SetActive(false);
        }
    }

    public void RollBuff()
    {
        GetRandomUniqueBuffs();
        _buffParent.SetActive(true);

    }

    public void OnReroll()
    {
        GetRandomUniqueBuffs();
        
        //if(_canRoll)
        //{
        //  _canRoll = false;
        //}    

    }

    public void OnCardBuffClicked(int index)
    {
        if (index < _randomBuffCards.Length)
        {
            Player._SPlayerScript.ApplyBuff(_randomBuffCards[index].statsType, _randomBuffCards[index].value);
        }
        _buffParent.SetActive(false);

    }

    private void GetRandomUniqueBuffs()
    {
        if (_buffCards.Length < _randomBuffCards.Length)
        {
            return;
        }

        Shuffle();

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
            _randomBuffCards[i].value = (float)Math.Round(UnityEngine.Random.Range(_buffCards[i].minValue, _buffCards[i].maxValue), 1);

            if (_randomBuffCards[i].valueHolder)
            {
                _randomBuffCards[i].valueHolder.text = "+ " + _randomBuffCards[i].value + "%";
            }

            _randomBuffCards[i].statsType = _buffCards[i].statsType;

        }

    }

    private void Shuffle()
    {
        System.Random rng = new System.Random();
        int n = _buffCards.Length;

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (_buffCards[k], _buffCards[n]) = (_buffCards[n], _buffCards[k]);
        }
    }
}

