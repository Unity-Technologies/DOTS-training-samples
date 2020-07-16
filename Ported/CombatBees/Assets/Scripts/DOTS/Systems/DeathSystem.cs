using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class DeathSystem : SystemBase
{
    private EntityQuery m_DeadResourcesQuery;
    private EntityQuery m_DeadBeesQuery;

    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_DeadResourcesQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<Dead>(),
                ComponentType.ReadOnly<Gravity>(),
                ComponentType.ReadOnly<Velocity>()
            }
        });

        m_DeadBeesQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<Dead>(),
                ComponentType.ReadOnly<Velocity>()
            },
            None = new[]
            {
                ComponentType.ReadOnly<Gravity>()
            }
        });

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        // Resources 
        //Entities.WithAll<Dead, Gravity>()
        //    .ForEach((int entityInQueryIndex, Entity entity) =>
        //    {
        //    });

        //// Bees
        //Entities.WithAll<Dead>()
        //    .WithNone<Gravity>()
        //    .ForEach((int entityInQueryIndex, Entity entity) => 
        //    { 
        //    });
            

        //var deadResources = m_DeadResourcesQuery.ToComponentDataArrayAsync<FieldInfo>(Unity.Collections.Allocator.TempJob, out var mainFieldHandle);

        //    Dependency = JobHandle.CombineDependencies(Dependency, mainFieldHandle);

        //    var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();

        //    Entities.WithAll<Gravity>()
        //        .WithDeallocateOnJobCompletion(mainField)
        //        .WithNone<Dead>()
        //        .ForEach((int entityInQueryIndex, Entity resourceEntity, ref Velocity v, in Translation t) =>
        //        {
        //            // Now check main field to see if resource should stop falling
        //            for (int j = 0; j < mainField.Length; ++j)
        //            {
        //                Bounds bound = mainField[j].Bounds;

        //                if (t.Value.y <= bound.Floor)
        //                    v.Value = new Unity.Mathematics.float3(0, 0, 0);
        //            }
        //        }).ScheduleParallel();

        //m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
