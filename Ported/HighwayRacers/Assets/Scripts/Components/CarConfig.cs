using Unity.Entities;

struct CarConfig : IComponentData
{
    public Entity CarPrefab;

    public float MinDefaultSpeed, MaxDefaultSpeed;
    public float MinOvertakeSpeedScale, MaxOvertakeSpeedScale;
    public float MinDistanceInFront, MaxDistanceInFront;
    public float MinMergeSpace, MaxMergeSpace;
    public float MinOvertakeEagerness, MaxOvertakeEagerness;
    public float MinLeftMergeDistance, MaxLeftMergeDistance;
}
