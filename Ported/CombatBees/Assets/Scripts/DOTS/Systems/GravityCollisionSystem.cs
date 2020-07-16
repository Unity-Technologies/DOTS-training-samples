using System.Diagnostics;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;

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
            .ForEach((int entityInQueryIndex, Entity entity, ref Velocity v, in LocalToWorld ltw, in RenderBounds entityBounds) =>
            {
                var t = ltw.Position;
                t.y -= entityBounds.Value.Extents.y;

                if (HasComponent<ResourceEntity>(entity))
                {
                    // Start with team fields to see if resources need to be flagged for death
                    for (int i = 0; i < teamFields.Length; ++i)
                    {
                        Bounds bounds = teamFields[i].Bounds;

                        if (bounds.Intersects(t, ignoreY: true) && (t.y <= bounds.Floor))
                        {
                            // tag resource for death here...
                            ecb.AddComponent(entityInQueryIndex, entity, new DespawnTimer { Time = 0.1f });
                            ecb.RemoveComponent<Gravity>(entityInQueryIndex, entity);
                            continue;
                        }
                    }
                }

                // Now handle main field collision
                for (int j = 0; j < mainField.Length; ++j)
                {
                    Bounds bound = mainField[j].Bounds;

                    var value = v.Value; 

                    if (t.y <= bound.Floor)
                    {
                        value = new float3(0, 0, 0);
                        ecb.RemoveComponent<Gravity>(entityInQueryIndex, entity);
                    }
                    else 
                    {
                        if (t.x <= bound.Min.x || t.x >= bound.Max.x)
                            value.x = 0;
                        if (t.z <= bound.Min.z || t.z >= bound.Max.z)
                            value.x = 0;
                    }

                    v.Value = value;
                }
            }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
