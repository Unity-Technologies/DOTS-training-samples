using Unity.Entities;
using Unity.Mathematics;

// An empty component is called a "tag component".
struct BucketInfo : IComponentData
{
    public float2 Position;
    public bool IsTaken;
    public bool IsFull;
}