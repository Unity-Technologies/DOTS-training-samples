using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct ResourceMovementSystem : ISystem
{
    private EntityQuery _resourcesQuery;
    private ComponentLookup<Resource> _resources;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // TODO filter out resources that are being carried by bees from gravity
        _resourcesQuery = SystemAPI.QueryBuilder().WithAll<Resource>().Build();
        _resources = state.GetComponentLookup<Resource>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var dt = SystemAPI.Time.DeltaTime;
        _resources.Update(ref state);
        
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);


        // Note currently there's nothing to do here any more because gravity has been moved to Physical.

        // var gravityJob = new GravityJob()
        // {
        //     Dt = dt,
        //     ECB = ecb,
        // };
        //
        // gravityJob.Schedule();
    }

    [BurstCompile]
    partial struct GravityJob : IJobEntity
    {
        public EntityCommandBuffer ECB;
        public float Dt;

        void Execute([EntityInQueryIndex] int index, Entity entity, ref Resource resource, ref Physical physical)
        {
            // TODO, if there's time make prettier looking animation that bobs with a slight delay

        }
    }
}