using Unity.Entities;

public struct PassengerSpawner : IComponentData
{
    public Entity platform;
    public Entity passengerPrefab;
    public int numQueues;
    public float queueSpacing;
    public float passengerSpacing;
    public int passengersPerQueue;
}