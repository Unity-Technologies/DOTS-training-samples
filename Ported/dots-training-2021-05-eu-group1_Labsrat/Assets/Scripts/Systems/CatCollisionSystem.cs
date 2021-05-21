using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;


[UpdateInGroup(typeof(ChuChuRocketUpdateGroup))]
public class CatCollisionSystem : SystemBase
{
    EntityCommandBufferSystem m_EcbSystem;
    AnimalSpawnerSystem m_AnimalSpawningSystem;
    NativeList<float2> m_CatPositions;
    
    protected override void OnCreate()
    {
        m_EcbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        m_AnimalSpawningSystem = World.GetExistingSystem<AnimalSpawnerSystem>();
        
        m_CatPositions = new NativeList<float2>(Allocator.Persistent);
    }
    
    protected override void OnUpdate()
    {
        if (TryGetSingleton(out GameConfig gameConfig))
        {
            NativeList<float2> catPositions = m_CatPositions;
            catPositions.Clear();
            
            Entities
                .WithAll<Cat>()
                .ForEach((in Translation translation) => 
                {
                    catPositions.Add(new float2(translation.Value.x, translation.Value.z));
                }).Schedule();

            EntityCommandBuffer.ParallelWriter ecb = m_EcbSystem.CreateCommandBuffer().AsParallelWriter();
            
            Entities
                .WithAll<Mouse>()
                .WithReadOnly(catPositions)
                .ForEach((Entity mouseEntity, int entityInQueryIndex, in Translation translation, in Direction direction) =>
                {
                    float2 mousePosition = new float2(translation.Value.x, translation.Value.z);
                    foreach (var catPosition in catPositions)
                    {
                        if (math.distancesq(mousePosition, catPosition) < 0.4f)
                        {
                            // Cat collides with mouse
                            ecb.DestroyEntity(entityInQueryIndex, mouseEntity);
                        }
                    }
                }).ScheduleParallel();
            
            m_EcbSystem.AddJobHandleForProducer(Dependency);
        }
    }

    protected override void OnDestroy()
    {
        m_CatPositions.Dispose();
    }
}
