using Unity.Entities;
using Unity.Mathematics;

public struct ObstacleBucketIndices : IBufferElementData
{
    public static implicit operator Entity(ObstacleBucketIndices e) { return e.Value; }
    public static implicit operator ObstacleBucketIndices(Entity e) { return new ObstacleBucketIndices { Value = e }; }
    
    public Entity Value;
}
