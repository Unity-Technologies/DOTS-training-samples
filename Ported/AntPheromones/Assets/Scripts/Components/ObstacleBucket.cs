using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct ObstacleBucket : IBufferElementData
{
    public static implicit operator Translation(ObstacleBucket e) { return e.Value; }
    public static implicit operator ObstacleBucket(Translation e) { return new ObstacleBucket { Value = e }; }

    public Translation Value;
}
