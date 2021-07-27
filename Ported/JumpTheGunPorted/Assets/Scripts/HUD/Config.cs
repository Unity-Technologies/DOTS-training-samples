using UnityEngine;

// This is a ref type so it can be modified by the HUD
public class ConfigData
{
    public int TerrainWidth;
    public int TerrainLength;

    public float MinTerrainHeight;
    public float MaxTerrainHeight;
    public float HeightDamage;

    public int TankCount;
    public float TankReloadTime;

    public bool Paused;
}

public class Config : MonoBehaviour
{
    public ConfigData Data { get; private set; }
    public static Config Instance { get; private set; }

    [Header("Defaults")]
    [SerializeField] private int _TerrainWidth = 10;
    [SerializeField] private int _TerrainLength = 10;

    [SerializeField] private float _MinTerrainHeight = 1f;
    [SerializeField] private float _MaxTerrainHeight = 10f;
    [SerializeField] private float _HeightDamage = 2f;

    [SerializeField] private int _TankCount = 10;
    [SerializeField] private float _TankReloadTime = 3f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Data = new ConfigData
        {
            TerrainWidth = _TerrainWidth,
            TerrainLength = _TerrainLength,
            MinTerrainHeight = _MinTerrainHeight,
            MaxTerrainHeight = _MaxTerrainHeight,
            HeightDamage = _HeightDamage,
            TankCount = _TankCount,
            TankReloadTime = _TankReloadTime
        };
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
