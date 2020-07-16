using System.Diagnostics;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

public class GravityCollisionSystem : SystemBase
{
    private EntityQuery m_MainFieldQuery;
    private EntityQuery m_TeamFieldsQuery;
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_MainFieldQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<FieldInfo>()
            },
            None = new[]
            {
                ComponentType.ReadOnly<TeamOne>(),
                ComponentType.ReadOnly<TeamTwo>()
            }
        });

        m_TeamFieldsQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<FieldInfo>()
            },
            Any = new[]
            {
                ComponentType.ReadOnly<TeamOne>(),
                ComponentType.ReadOnly<TeamTwo>(),
            }
        });

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var mainField = m_MainFieldQuery.ToComponentDataArrayAsync<FieldInfo>(Unity.Collections.Allocator.TempJob, out var mainFieldHandle);
        var teamFields = m_TeamFieldsQuery.ToComponentDataArrayAsync<FieldInfo>(Unity.Collections.Allocator.TempJob, out var teamFieldsHandle);

        Dependency = JobHandle.CombineDependencies(Dependency, mainFieldHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, teamFieldsHandle);

        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();

        Entities.WithAll<Gravity>()
            .WithDeallocateOnJobCompletion(mainField)
            .WithDeallocateOnJobCompletion(teamFields)
            .WithNone<Dead>()
            .ForEach((int entityInQueryIndex, Entity resourceEntity, ref Velocity v, in LocalToWorld ltw) =>
            {
                var t = ltw.Position;

                // Start with team fields to see if resources need to be flagged for death
                for (int i = 0; i < teamFields.Length; ++i)
                {
                    Bounds bounds = teamFields[i].Bounds;

                    if (bounds.Intersects(t, ignoreY:true) && (t.y <= bounds.Floor))
                    {
                        // tag resource for death here...
                        ecb.AddComponent<Dead>(entityInQueryIndex, resourceEntity);
                        v.Value = new float3(0, 0, 0);
                        continue;
                    }
                }
               
                // Now check main field to see if resource should stop falling
                for (int j = 0; j < mainField.Length; ++j)
                {
                    Bounds bound = mainField[j].Bounds;

                    if (bound.Intersects(t, ignoreY: true) && t.y <= bound.Floor)
                        v.Value = new float3(0, 0, 0);
                }
            }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
