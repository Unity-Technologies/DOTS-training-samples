using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;

public struct Config : IComponentData
{
    public Entity tankPrefab;
    public Entity boxPrefab;
    public Entity playerPrefab;

    public const float minHeight = 0.5f;
    public const float yOffset = 1f;

    public float terrainWidth;
    public int terrainLength;
    public float minTerrainHeight;
    public float maxTerrainHeight;

    public static readonly float4 minHeightColour = new float4(0, 1f, 0, 1f);
    public static readonly float4 maxHeightColour = new float4(99 / 255f, 47 / 255f, 0 / 255f, 1.0f);

    public int boxHeightDamage;
    public int boxCount;

    public int tankCount;

    public float tankLaunchPeriod;
    public float collisionStepMultiplier;
    public float invPlayerParabolaPrecision;
    public float spacing;

    public bool invincibility;
    public bool isPaused;

    public bool isSetUp; 


    // Probably should be in a different script but I'm lazy :(( 
    public float3 rayDirection;
    public float3 rayOrigin; 

    public FixedString64Bytes timeText;
}
