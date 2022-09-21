using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[WithAll(typeof(BeeConfig))] // creative way to limit to 1 instance of job at a time!
partial struct ResourceConsumptionJob : IJobEntity
{
    public EntityCommandBuffer ECB;

    public int beeBatchSize;
    public Entity beePrefab;

    void Execute(Entity resource)
    {
        NativeArray<Entity> newBees = new NativeArray<Entity>(beeBatchSize, Allocator.Temp);
        ECB.Instantiate(beePrefab, newBees);
    }
}

partial struct ResourceConsumptionSystem : ISystem
{
    private double nextConsumptionTime;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ResourceConfig>();
        state.RequireForUpdate<BeeConfig>();

        nextConsumptionTime = 0;
    }

    public void OnDestroy(ref SystemState state)
    {
        throw new System.NotImplementedException();
    }

    public void OnUpdate(ref SystemState state)
    {
        // TODO detect consumption events... for now, N new bees per second!
        if (state.Time.ElapsedTime < nextConsumptionTime)
            return;
        nextConsumptionTime = state.Time.ElapsedTime + 1;

        var resourceConfig = SystemAPI.GetSingleton<ResourceConfig>();
        var beeConfig = SystemAPI.GetSingleton<BeeConfig>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        new ResourceConsumptionJob()
            { ECB = ecb, beeBatchSize = resourceConfig.beesPerResource, beePrefab = beeConfig.beePrefab }.Schedule();
    }
}