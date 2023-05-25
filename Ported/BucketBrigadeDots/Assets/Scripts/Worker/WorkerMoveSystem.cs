using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateBefore(typeof(TransformSystemGroup)), UpdateAfter(typeof(TeamUpdateSystem))]
public partial struct WorkerMoveSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var moveJob = new MoveEntityJob()
        {
            cmdBuffer = state.World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>()
                .CreateCommandBuffer().AsParallelWriter(),
            deltaTime = SystemAPI.Time.DeltaTime
        };
        state.Dependency = moveJob.ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    partial struct MoveEntityJob : IJobEntity
    {
        public float deltaTime;
        public EntityCommandBuffer.ParallelWriter cmdBuffer;
        
        public void Execute(in NextPosition nextPosition, ref LocalTransform transform, in Entity entity)
        {
            var target = nextPosition.Value;
            if (Movement.MoveToPosition(ref target, ref transform, deltaTime))
                cmdBuffer.SetComponentEnabled<NextPosition>(entity.Index, entity, false);
        }
    }
}