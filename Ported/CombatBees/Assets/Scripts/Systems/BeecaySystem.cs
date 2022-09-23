using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[UpdateAfter(typeof(GroundImpactSystem))]
partial struct BeecaySystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new DecayVelocityJob().ScheduleParallel();
        
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
        new BeeDespawnJob() { ECB = ecb, DeltaTime = state.Time.DeltaTime }.ScheduleParallel();
        new ResourceDespawnJob() { ECB = ecb }.ScheduleParallel();
    }
}