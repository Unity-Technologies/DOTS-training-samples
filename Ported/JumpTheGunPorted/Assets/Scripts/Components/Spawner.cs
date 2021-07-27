using Unity.Entities;

// When a component doesn't require any special handling during conversion,
// the attribute GenerateAuthoringComponent can be used to automatically
// generate an authoring component with the same fields and a conversion
// function that simply copies the fields from one to the other.
[GenerateAuthoringComponent]
public struct Spawner : IComponentData
{
    public Entity BoxPrefab;
    public Entity PlayerPrefab;
    public Entity TankPrefab;

    public int TerrainLength;
    public int TerrainWidth;

    public float MinTerrainHeight;
    public float MaxTerrainHeight;
    public float BoxHeightDamage;

    public int NumOfTanks;
    public float TankLaunchPeriod;
    public float CollisionStepMultiplier;
    public float PlayerParabolaPrecision;
}