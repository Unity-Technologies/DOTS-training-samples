using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

//[BurstCompile]
[WithAll(typeof(BeeConfig))] // creative way to limit to 1 instance of job at a time!
partial struct ResourceConsumptionJob : IJobEntity
{
    public EntityCommandBuffer ECB;

    public int beeBatchSize;

    void Execute(Entity resource)
    {
        var blueBees  = CollectionHelper.CreateNativeArray<Entity>(beeBatchSize / 2, Allocator.Temp);
        var yellowBees  = CollectionHelper.CreateNativeArray<Entity>(beeBatchSize / 2, Allocator.Temp);
        
        BeeSpawnHelper.SpawnBees(ECB, ref blueBees, BeeTeam.Blue, float2.zero);
        BeeSpawnHelper.SpawnBees(ECB, ref yellowBees, BeeTeam.Yellow, float2.zero);
    }
}

[UpdateAfter(typeof(GroundImpactSystem))]
partial struct ResourceConsumptionSystem : ISystem
{
    private double nextConsumptionTime;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ResourceConfig>();

        nextConsumptionTime = 0;
    }

    public void OnDestroy(ref SystemState state)
    {
        
    }

    public void OnUpdate(ref SystemState state)
    {
        // TODO detect consumption events... for now, N new bees per second!
        if (state.Time.ElapsedTime < nextConsumptionTime)
            return;
        nextConsumptionTime = state.Time.ElapsedTime + 1;

        var resourceConfig = SystemAPI.GetSingleton<ResourceConfig>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        new ResourceConsumptionJob()
            { ECB = ecb, beeBatchSize = resourceConfig.beesPerResource }.Schedule();
    }
}