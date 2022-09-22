using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[WithNone(typeof(Dead))]
[BurstCompile]
partial struct CombatJob : IJobEntity
{
    [ReadOnly] public ComponentLookup<LocalToWorldTransform> transformLookup;
    public EntityCommandBuffer.ParallelWriter ECB;
        
    public float attackDistance;
    
    [BurstCompile]
    void Execute([ChunkIndexInQuery] int chunkIndex, ref Velocity velocity, ref BeeProperties beeProperties, in LocalToWorldTransform localToWorldTransform)
    {
        if (beeProperties.BeeMode == BeeMode.Attack)
        {
            if (!transformLookup.HasComponent(beeProperties.Target))
            {
                beeProperties = new BeeProperties
                {
                    BeeMode = BeeMode.Idle,
                    Aggressivity = beeProperties.Aggressivity,
                    Target = Entity.Null,
                    TargetPosition = float3.zero,
                };
            }
            else
            {
                var beePosition = localToWorldTransform.Value.Position;
                var targetPosition = transformLookup[beeProperties.Target].Value.Position;

                if (math.distancesq(beePosition, targetPosition) < attackDistance)
                {
                    velocity = new Velocity { Value = targetPosition - beePosition };
                    ECB.SetComponentEnabled<Dead>(chunkIndex, beeProperties.Target, true);
                }
            }
        }
    }
}

[UpdateBefore(typeof(MovementSystem))]
[UpdateBefore(typeof(BeeSystem))]
[BurstCompile]
partial struct CombatSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeeConfig>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var transformLookup = SystemAPI.GetComponentLookup<LocalToWorldTransform>(true);
        
        var combatJob = new CombatJob
        {
            ECB = ECB.AsParallelWriter(),
            transformLookup = transformLookup,
            attackDistance = 4.0f
        };
        combatJob.ScheduleParallel();
    }
}