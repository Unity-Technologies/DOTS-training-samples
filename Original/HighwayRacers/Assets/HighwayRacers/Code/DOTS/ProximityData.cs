using Unity.Entities;

public struct ProximityData : IComponentData
{
    public float NearestFrontMyLane;
    public float NearestFrontMyLaneSpeed;
    public float NearestFrontRight;
    public float NearestFrontRightSpeed;
    public float NearestFrontLeft;
    public float NearestRearRight;
    public float NearestRearLeft;

}
