using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct DestinationAspect : IAspect
{
    private const float defaultEpsilon = 0.1f;
    public readonly TransformAspect transform;
    public readonly RefRO<TargetDestination> target;

    [BurstCompile]
    public bool IsAtDestination(float epsilon = defaultEpsilon)
    {
        return math.distancesq(transform.WorldPosition, target.ValueRO.TargetPosition) < defaultEpsilon;
    }
}
