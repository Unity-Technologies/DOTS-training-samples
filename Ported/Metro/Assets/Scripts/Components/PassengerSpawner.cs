using Unity.Entities;

public struct PassengerSpawner : IComponentData
{
    public Entity PassengerPrefab;
    public int TotalCount;
}
