using Unity.Entities;


struct CarAICache : IComponentData
{
    public Entity CarInFront;
    public float CarInFrontSpeed;
    public bool CanMergeRight;
    public bool CanMergeLeft;
    public float DistanceAhead;
    public float DistanceBehind;
}