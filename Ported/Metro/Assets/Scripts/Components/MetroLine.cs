using Unity.Collections;
using Unity.Entities;

public struct MetroLine : IComponentData
{
    public NativeArray<Entity> RailwayPoints;
    public NativeArray<Entity> Platforms;
}