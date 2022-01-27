using Unity.Entities;

public struct CarSpawner : IComponentData
{
    public Entity carPrefab;
    public int carPerRoad;
    public float carSpeed;
    public float carLength;
}
