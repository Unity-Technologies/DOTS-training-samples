using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public class PathData : IComponentData
{
    public NativeArray<float3> Positions;
    public NativeArray<int> MarkerType;
    public NativeArray<float3> HandlesIn;
    public NativeArray<float3> HandlesOut;
    public NativeArray<float> Distances;
    public float TotalDistance;
}