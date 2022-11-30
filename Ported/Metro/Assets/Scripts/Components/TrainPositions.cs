using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct TrainPositions : IComponentData
{
    public NativeArray<Entity> Trains;
    public NativeArray<float3> TrainsPositions;
    public NativeArray<quaternion> TrainsRotations;
    public NativeArray<int> StartIndexForMetroLine;
}