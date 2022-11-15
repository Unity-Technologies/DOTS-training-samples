using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[BurstCompile]
public partial struct PhysicalSystem : ISystem
{
    private EntityQuery _physicalQuery;
    private ComponentLookup<Physical> _physicals;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _physicalQuery = SystemAPI.QueryBuilder().WithAll<Physical>().Build();
        _physicals = state.GetComponentLookup<Physical>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var dt = SystemAPI.Time.DeltaTime;
        _physicals.Update(ref state);

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var physicalJob = new PhysicalJob()
        {
            Dt = dt,
            ECB = ecb,
        };

        physicalJob.Schedule();
    }

    [BurstCompile]
    partial struct PhysicalJob : IJobEntity
    {
        public EntityCommandBuffer ECB;
        public float Dt;

        void Execute([EntityInQueryIndex] int index, Entity entity, ref Physical physical)
        {
            physical.Position += physical.Velocity * Dt;

            var uniformScaleTransform = new UniformScaleTransform
            {
                Position = physical.Position,
                // Maybe we need something here to rotate in the direction of movement - TJA
                Rotation = quaternion.identity,
                Scale = 1
            };

            ECB.SetComponent(entity, new LocalToWorldTransform
            {
                Value = uniformScaleTransform
            });

        }
    }

}