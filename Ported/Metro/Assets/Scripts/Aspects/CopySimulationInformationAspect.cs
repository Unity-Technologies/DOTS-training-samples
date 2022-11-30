using Unity.Entities;
using Unity.Transforms;

readonly partial struct CopyTrainPositionAspect : IAspect
{
    readonly TransformAspect Transform;
    
    public readonly RefRW<Train> Train;
}