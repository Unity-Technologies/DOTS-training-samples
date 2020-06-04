using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct SpawnerInfo : IComponentData
{
    public Entity Prefab;
    public float2 WalkSpeed;
    public float RotationSpeed;

    public Entity AlternatePrefab;
    public float2 AlternateWalkSpeed;
    public float AlternateRotationSpeed;

    public float MaxSpawns;
    public float Frequency;
    public float AlternateSpawnMinFrequency;
    public float AlternateSpawnMaxFrequency;
}