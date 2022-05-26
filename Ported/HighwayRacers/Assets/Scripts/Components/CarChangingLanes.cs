using Unity.Entities;

struct CarChangingLanes : IComponentData
{
    public int FromLane;
    public int ToLane;
    public float Progress;
}