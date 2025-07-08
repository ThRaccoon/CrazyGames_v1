using UnityEngine;

public class GlobalTimer
{
    private bool _flag = false;
    #region Getter / Setter
    public bool Flag
    {
        get => _flag;
        set => _flag = value;
    }
    #endregion

    private float _duration = 0f;
    #region Getter / Setter
    public float Duration
    {
        get => _duration;
        set => _duration = value;
    }
    #endregion

    private float _elapsedTime = 0f;
    #region Getter / Setter
    public float ElapsedTime
    {
        get => _elapsedTime;
        set => _elapsedTime = value;
    }
    #endregion

    public GlobalTimer(float duration, bool flag = false)
    {
        _duration = duration;
        _flag = flag;

        _elapsedTime = _duration;
    }

    public bool Tick()
    {
        if (_flag) return _flag;

        _elapsedTime -= Time.deltaTime;

        if (_elapsedTime <= 0)
        {
            _flag = true;
            _elapsedTime = 0f;
        }

        return _flag;
    }

    public void Reset()
    {
        _elapsedTime = _duration;
        _flag = false;
    }

    public bool TickReversed()
    {
        if (!_flag) return _flag;

        _elapsedTime -= Time.deltaTime;

        if (_elapsedTime <= 0)
        {
            _flag = false;
            _elapsedTime = 0f;
        }

        return _flag;
    }

    public void ResetReversed()
    {
        _elapsedTime = _duration;
        _flag = true;
    }
}