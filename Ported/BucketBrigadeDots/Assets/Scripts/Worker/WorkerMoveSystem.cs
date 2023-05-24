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
            var target = nextPosition.ValueRO.Value;
            if (Movement.MoveToPosition(ref target, ref transform.ValueRW, SystemAPI.Time.DeltaTime))
            {
                SystemAPI.SetComponentEnabled<NextPosition>(entity, false);
            }
        }
    }
}