using Unity.Entities;

public struct PassengersConfig : IComponentData
{
    public Entity passengerPrefab;
    public int passengerCount;
}
