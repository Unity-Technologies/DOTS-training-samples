using Unity.Entities;
using Unity.Collections;

class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject tankPrefab;
    public UnityEngine.GameObject boxPrefab;
    public UnityEngine.GameObject playerPrefab;

    public int tankCount;

    public float terrainWidth;
    public int terrainLength;
    public float minTerrainHeight;
    public float maxTerrainHeight;

    public int boxCount;
    public int boxHeightDamage;
    public float tankLaunchPeriod;
    public float collisionStepMultiplier;
    public float invPlayerParabolaPrecision;
    public float spacing;
    public bool setUp; 

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

            boxPrefab = GetEntity(authoring.boxPrefab),
            boxCount = authoring.boxCount,
            boxHeightDamage = authoring.boxHeightDamage,

            tankLaunchPeriod = authoring.tankLaunchPeriod,
            collisionStepMultiplier = authoring.collisionStepMultiplier,
            invPlayerParabolaPrecision = authoring.invPlayerParabolaPrecision,
            spacing = authoring.spacing,

            playerPrefab = GetEntity(authoring.playerPrefab),
            invincibility = authoring.invincibility,
            isPaused = authoring.isPaused,
            isSetUp = authoring.setUp,
            timeText = authoring.timeText,

        });
    }
}