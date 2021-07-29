using UnityEngine;

public struct ConfigData
{
    public int TerrainWidth;
    public int TerrainLength;

    public float MinTerrainHeight;
    public float MaxTerrainHeight;
    public float HeightDamage;

    public int TankCount;
    public float TankReloadTime;

    public float PlayerParabolaPrecision;
    public float CollisionStepMultiplier;

    public bool Paused;
    public bool Invincible;
}

public class Config : MonoBehaviour
{
    private ConfigData _Data;
    public ConfigData Data
    {
        get => _Data;
        set => _Data = value;
    }
    
    public static Config Instance { get; private set; }

    [Header("Defaults")]
    [SerializeField] private int _TerrainWidth = 15;
    [SerializeField] private int _TerrainLength = 10;

    [SerializeField] private float _MinTerrainHeight = 2.5f;
    [SerializeField] private float _MaxTerrainHeight = 5.5f;
    [SerializeField] private float _HeightDamage = 0.4f;

    [SerializeField] private int _TankCount = 5;
    [SerializeField] private float _TankReloadTime = 3f;

    [SerializeField] private int _CollisionStepMultiplier = 4;
    [SerializeField] private float _PlayerParabolaPrecision = 0.1f;

    [SerializeField] private bool _Invincible = false;

    public float TimeVal { get; set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _Data = CreateConfigData();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public ConfigData CreateConfigData()
    {
        return new ConfigData()
        {
            TerrainWidth = _TerrainWidth,
            TerrainLength = _TerrainLength,
            MinTerrainHeight = _MinTerrainHeight,
            MaxTerrainHeight = _MaxTerrainHeight,
            HeightDamage = _HeightDamage,
            TankCount = _TankCount,
            TankReloadTime = _TankReloadTime,
            PlayerParabolaPrecision = _PlayerParabolaPrecision,
            CollisionStepMultiplier = _CollisionStepMultiplier,
            Invincible = _Invincible,
            Paused = false
        };
    }

    public void TogglePause()
    {
        _Data.Paused = !_Data.Paused;
    }

    void Update()
    {
        if (!_Data.Paused)
        {
            TimeVal += Time.deltaTime;
        }
    }
}
