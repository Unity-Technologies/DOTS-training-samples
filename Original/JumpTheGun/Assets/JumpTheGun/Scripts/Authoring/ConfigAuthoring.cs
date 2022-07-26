using Unity.Entities;

class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject tankPrefab;
    public int tankCount;

    public float terrainWidth;
    public int terrainLength;
    public float minTerrainHeight;
    public float maxTerrainHeight;

    public int boxHeightDamage;
    public float tankLaunchPeriod;
    public float collisionStepMultiplier;
    public float invPlayerParabolaPrecision;

    public bool invincibility;
    public bool isPaused;

    public string timeText;
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config
        {
            tankPrefab = GetEntity(authoring.tankPrefab),
            tankCount = authoring.tankCount,
            terrainWidth = authoring.terrainWidth,
            terrainLength = authoring.terrainLength,
            minTerrainHeight = authoring.minTerrainHeight,
            maxTerrainHeight = authoring.maxTerrainHeight,
            boxHeightDamage = authoring.boxHeightDamage,
            tankLaunchPeriod = authoring.tankLaunchPeriod,
            collisionStepMultiplier = authoring.collisionStepMultiplier,
            invPlayerParabolaPrecision = authoring.invPlayerParabolaPrecision,
            invincibility = authoring.invincibility,
            isPaused = authoring.isPaused,
            timeText = authoring.timeText,
        });
    }
}