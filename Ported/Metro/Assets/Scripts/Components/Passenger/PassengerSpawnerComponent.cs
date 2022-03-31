using Unity.Entities;

public struct PassengerSpawnerComponent : IComponentData
{
    public Entity PassengerPrefab;
    public int PassengersPerStation;
}
