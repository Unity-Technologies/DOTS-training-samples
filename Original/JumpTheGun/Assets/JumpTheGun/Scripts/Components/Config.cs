using Unity.Entities;
using Unity.Collections;

struct Config : IComponentData
{
    public Entity tankPrefab;

    public float terrainWidth;
    public int terrainLength;
    public float minTerrainHeight;
    public float maxTerrainHeight;

    public int boxHeightDamage;
    public int tankCount;
    public float tankLaunchPeriod;
    public float collisionStepMultiplier;
    public float invPlayerParabolaPrecision;

    public bool invincibility;
    public bool isPaused;
    public string timeText;
}
