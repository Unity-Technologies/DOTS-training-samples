

using Unity.Entities;

[GenerateAuthoringComponent]
public struct Config : IComponentData
{
    public float MouseMovementSpeed;
    public float CatMovementSpeed;

    public float MouseSpawnRate;

    public uint MapSeed;
    
    public Entity CatPrefab;
    public Entity MousePrefab;
    public Entity ExitPrefab;
    public Entity ArrowPrefab;
}
