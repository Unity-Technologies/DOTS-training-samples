using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
[UpdateAfter(typeof(TargetingSystem))]
partial struct AttackingSystem : ISystem {
    // readonly for burst
    public static readonly float chaseForce = 50f;
    public static readonly float hitDistance = 0.5f;
    public static readonly float attackForce = 500f;
    public static readonly float attackDistance = 4f;
    
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
        state.Dependency = new AttackingJob() {
            deltaTime = state.Time.DeltaTime,
            chaseForce = chaseForce,
            attackForce = attackForce,
            attackDistanceSquared = attackDistance*attackDistance,
            hitDistanceSquared = hitDistance * hitDistance,
            transformLookup = state.GetComponentLookup<LocalToWorldTransform>(),
            ecb = ecb
        }.ScheduleParallel(state.Dependency);
    }
}