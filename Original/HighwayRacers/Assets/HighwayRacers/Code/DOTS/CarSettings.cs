using Unity.Entities;

struct CarSettings : IComponentData
{
    public float DefaultSpeed;
    public float OvertakePercent;
    public float LeftMergeDistance;
    public float MergeSpace;
    public float OvertakeEagerness;
}

