[System.Serializable]
public class PlayerData
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // Profile
    private int _lvl = 1;
    #region Getter / Setter
    public int Level
    {
        get => _lvl;
        set { _lvl = value; }
    }
    #endregion

    private int _xp = 0;
    #region Getter / Setter
    public int XP
    {
        get => _xp;
        set { _xp = value; }
    }
    #endregion
    ////////////////////////////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // Currencies
    private int _shards = 0;
    #region Getter / Setter
    public int Shards
    {
        get => _shards;
        set { _shards = value; }
    }
    #endregion

    private int _gems = 0;
    #region Getter / Setter
    public int Gems
    {
        get => _gems;
        set { _gems = value; }
    }
    #endregion
    ////////////////////////////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // Stats
    private float _attackDamagePoints = 0f;
    #region Getter / Setter
    public float AttackDamagePoints
    {
        get => _attackDamagePoints;
        set { _attackDamagePoints = value; }
    }
    #endregion

    private float _critChancePoints = 0f;
    #region Getter / Setter
    public float CritChancePoints
    {
        get => _critChancePoints;
        set { _critChancePoints = value; }
    }
    #endregion

    private float _defensePoints = 0f;
    #region Getter / Setter
    public float DefensePoints
    {
        get => _defensePoints;
        set { _defensePoints = value; }
    }
    #endregion

    private float _healthPoints = 0f;
    #region Getter / Setter
    public float HealthPoints
    {
        get => _healthPoints;
        set { _healthPoints = value; }
    }
    #endregion

    private float _healthRegenAmountPoints = 0f;
    #region Getter / Setter
    public float HealthRegenAmountPoints
    {
        get => _healthRegenAmountPoints;
        set { _healthRegenAmountPoints = value; }
    }
    #endregion
    ////////////////////////////////////////////////////////////////////////////////////////////////////
}