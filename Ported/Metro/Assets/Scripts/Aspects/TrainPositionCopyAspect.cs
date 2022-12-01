using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

readonly partial struct TrainPositionCopyAspect : IAspect
{
    // Aspects can contain other aspects.
    public readonly RefRO<WorldTransform> Transform;
    
    public readonly RefRO<UniqueTrainID> Train;

    public int ID => Train.ValueRO.ID;
    public float3 Position => Transform.ValueRO.Position;
    public quaternion Rotation => Transform.ValueRO.Rotation;
}