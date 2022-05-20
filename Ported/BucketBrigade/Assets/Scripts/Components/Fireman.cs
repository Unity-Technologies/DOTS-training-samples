using Unity.Entities;
using Unity.Mathematics;

//tag component for firemen designated to pass around empty buckets
struct EmptyBucketPasser : IComponentData
{

}

//tag component for firemen designated to pass around filled buckets
struct FilledBucketPasser : IComponentData
{

}

public enum FiremanState
{
    Awaiting,
    OnRouteToDestination,
    Stopped,
}

struct Fireman : IComponentData
{
    public float3 Destination;
    public float Speed;
    public float SearchRadius;
    public FiremanState State;
}
