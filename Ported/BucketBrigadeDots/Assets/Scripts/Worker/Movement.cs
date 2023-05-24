using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;


[BurstCompile]
class Movement {
    // Returns true if we've reached the target position.
    [BurstCompile]
    public static bool MoveToPosition(ref float2 targetPosition, ref LocalTransform transform, float deltaTime)
    {
        var delta = targetPosition - transform.Position.xz;
        var length = math.length(delta);
        
        if (length <= 0.1f) return true;

        var direction = math.normalize(delta);

        var moveAmount = direction * 10f * deltaTime;
        transform.Position += new float3(moveAmount.x, 0f, moveAmount.y);
        return false;
    }
}