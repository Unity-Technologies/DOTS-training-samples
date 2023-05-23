using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(TeamUpdateSystem))]
public partial struct WorkerMoveSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (nextPosition, transform, entity) in SystemAPI.Query<
                         RefRO<NextPosition>, 
                         RefRW<LocalTransform>>()
                     .WithEntityAccess())
        {
            var delta = nextPosition.ValueRO.Value - transform.ValueRW.Position.xz;
            var length = math.length(delta);
            if (length > 0.1f)
            {
                var direction = math.normalize(delta);

                var moveAmount = direction * 10f * SystemAPI.Time.DeltaTime;
                transform.ValueRW.Position += new float3(moveAmount.x, 0f, moveAmount.y);
            }
            else
                SystemAPI.SetComponentEnabled<NextPosition>(entity, false);
        }
    }
}