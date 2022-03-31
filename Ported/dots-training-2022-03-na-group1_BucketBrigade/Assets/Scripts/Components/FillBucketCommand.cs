using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(32)]
public struct FillBucketCommand : IBufferElementData
{
    public Entity Worker;
    public Entity WaterPool;
    
    public FillBucketCommand(Entity worker, Entity waterPool)
    {
        Worker = worker;
        WaterPool = waterPool;
    }
}