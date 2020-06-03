using Unity.Entities;

[GenerateAuthoringComponent]
public struct CarSpawner : IComponentData
{
    public int InstancesToSpawn;
    public Entity CarPrefab;

    public float MaxDefaultSpeed;
    public float MaxOvertakeSpeed;
    public float MaxDistanceToCarBeforeOvertaking;
    public float MaxOvertakeEagerness;
    public float MaxMergeSpace;
}