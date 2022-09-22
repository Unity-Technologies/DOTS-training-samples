using Unity.Entities;
using Unity.Transforms;

[UpdateAfter(typeof(TargetingSystem))]
partial struct AttackingSystem : ISystem {
    public static float chaseForce = 50f;
    public static float hitDistance = 0.5f;
    public static float attackForce = 500f;
    public static float attackDistance = 4f;
    
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

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