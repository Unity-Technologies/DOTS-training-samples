using Unity.Collections;
using Unity.Entities;

public struct BucketComponentData : IComponentData
{
    public float minRange;
    public float maxRange;
}