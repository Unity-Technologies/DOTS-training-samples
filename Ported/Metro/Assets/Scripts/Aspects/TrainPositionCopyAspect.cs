using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

readonly partial struct TrainPositionCopyAspect : IAspect
{
    // Aspects can contain other aspects.
    public readonly RefRO<WorldTransform> Transform;
    
    public readonly RefRO<Train> Train;

    public int ID => Train.ValueRO.UniqueTrainID;
    public float3 Position => Transform.ValueRO.Position;
    public quaternion Rotation => Transform.ValueRO.Rotation;
}