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
        _resourcesQuery = state.GetEntityQuery(typeof(Resource));
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

        var gravityJob = new GravityJob()
        {
            Dt = dt,
            ECB = ecb,
        };

        gravityJob.Schedule();

        // resourceComponents.Dispose(state.Dependency);
    }

    [BurstCompile]
    partial struct GravityJob : IJobEntity
    {
        public EntityCommandBuffer ECB;
        public float Dt;

        void Execute([EntityInQueryIndex] int index, Entity entity, ref Resource resource, ref Physical physical)
        {
            // // TODO, if there's time make prettier looking animation that bobs with a slight delay
            physical.Position = ApplyGravity(physical.Position, Dt);
            var uniformScaleTransform = new UniformScaleTransform
            {
                // ALX: now using Field bounds, but could cluster closer to the centre if desired
                Position = ApplyGravity(physical.Position, Dt),
                Rotation = quaternion.identity,
                Scale = 1
            };
            
            ECB.SetComponent(entity, new LocalToWorldTransform
            {
                Value = uniformScaleTransform
            });
        }

        /// <summary>
        /// Only linear gravity right now. Potentially TODO have acceleration
        /// </summary>
        /// <param name="position"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        static float3 ApplyGravity(float3 position, float dt)
        {
            var newPosition = position;
            newPosition.y += dt * Field.gravity; // ALX: using field from static class, consider using a baked entity config instead
            newPosition.y = math.max(newPosition.y, Field.GroundLevel);
            return newPosition;
        }
    }
}
