using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class ParSpawnSystem : JobComponentSystem
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        // Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    struct SpawnJob : IJobForEachWithEntity<ParSpawnRuntime, LocalToWorld>
    {
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public void Execute(Entity entity, int index, [ReadOnly] ref ParSpawnRuntime particleSpawnerComponent,
            [ReadOnly] ref LocalToWorld location)
        {
            Random random = new Random(7777);
            
            for (var x = 0; x < particleSpawnerComponent.SharkCount; x++)
            {
                var instance = CommandBuffer.Instantiate(index, particleSpawnerComponent.Prefab);

                // Place the instantiated in a grid with some noise
                float3 position = float3(random.NextFloat(-50.0f, 50.0f), random.NextFloat(0.0f, 50.0f), random.NextFloat(-50.0f, 50.0f));
                
                // Color?
                // colors[i] = Color.white * Random.Range(.3f,.7f);
                
                // Set components
                CommandBuffer.SetComponent(index, instance, new Translation { Value = position });
                CommandBuffer.SetComponent(index, instance, new ParticleComponent { RadiusMult = random.NextFloat() });
            }

            CommandBuffer.DestroyEntity(index, entity);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        // Schedule the job that will add Instantiate commands to the EntityCommandBuffer.
        var job = new SpawnJob
        {
            CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        }.Schedule(this, inputDeps);
        
        // SpawnJob runs in parallel with no sync point until the barrier system executes.
        m_EntityCommandBufferSystem.AddJobHandleForProducer(job);
        return job;
    }
}
