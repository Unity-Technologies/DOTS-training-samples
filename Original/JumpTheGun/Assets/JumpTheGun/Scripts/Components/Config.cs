using Unity.Entities;
using Unity.Collections;

public struct Config : IComponentData
{
    public Entity tankPrefab;
    public Entity boxPrefab;

    public float terrainWidth;
    public int terrainLength;
    public float minTerrainHeight;
    public float maxTerrainHeight;

    public int boxHeightDamage;
    public int boxCount;

    public int tankCount;

    public float tankLaunchPeriod;
    public float collisionStepMultiplier;
    public float invPlayerParabolaPrecision;
    public float spacing;

    public bool invincibility;
    public bool isPaused;

    public FixedString64Bytes timeText;
}
