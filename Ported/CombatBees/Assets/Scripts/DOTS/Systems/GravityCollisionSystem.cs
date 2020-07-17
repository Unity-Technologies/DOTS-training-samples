using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;

public class GravityCollisionSystem : SystemBase
{
    private EntityQuery m_MainFieldQuery;
    private EntityQuery m_TeamFieldsQuery;
    private EntityQuery m_BeeSpawnerQuery;
    private EntityCommandBufferSystem m_ECBSystem;
    Unity.Mathematics.Random m_Random = new Unity.Mathematics.Random(0x5716318);

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

        m_BeeSpawnerQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<BeeSpawner>()
            }
        });

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var mainField = m_MainFieldQuery.ToComponentDataArrayAsync<FieldInfo>(Unity.Collections.Allocator.TempJob, out var mainFieldHandle);
        var teamFields = m_TeamFieldsQuery.ToComponentDataArrayAsync<FieldInfo>(Unity.Collections.Allocator.TempJob, out var teamFieldsHandle);
        var beeSpawners = m_BeeSpawnerQuery.ToComponentDataArrayAsync<BeeSpawner>(Unity.Collections.Allocator.TempJob, out var beeSpawnersHandle);

        Dependency = JobHandle.CombineDependencies(Dependency, mainFieldHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, teamFieldsHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, beeSpawnersHandle);

        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();
        var random = m_Random;

        var minBeeSize = BeeManager.Instance.minBeeSize;
        var maxBeeSize = BeeManager.Instance.maxBeeSize;

        Dependency = Entities.WithAll<Gravity>()
            .WithNone<Carried>()
            .WithDeallocateOnJobCompletion(mainField)
            .WithDeallocateOnJobCompletion(teamFields)
            .WithDeallocateOnJobCompletion(beeSpawners)
            .ForEach((int entityInQueryIndex, Entity entity, ref Velocity v, ref Translation translation, in RenderBounds entityBounds) =>
            {
                var t = translation.Value;
                t.y -= entityBounds.Value.Extents.y;

                if (HasComponent<ResourceEntity>(entity))
                {
                    // Start with team fields to see if resources need to be flagged for death
                    for (int i = 0; i < teamFields.Length; ++i)
                    {
                        Bounds bounds = teamFields[i].Bounds;

                        if (bounds.Intersects(t, ignoreY: true) && (t.y <= bounds.Floor))
                        {
                            // tag resource for death
                            ecb.AddComponent(entityInQueryIndex, entity, new DespawnTimer { Time = 0.1f });
                            ecb.RemoveComponent<Gravity>(entityInQueryIndex, entity);

                            // spawn bees - this code is a bit hacky but we're out of time.
                            if (beeSpawners.Length > 0)
                            {
                                var spawner = beeSpawners[0];

                                for (int n = 0; n < spawner.BeesPerResource; ++n)
                                {
                                    var instance = ecb.Instantiate(entityInQueryIndex, spawner.Prefab);

                                    ecb.SetComponent(entityInQueryIndex,
                                        instance,
                                        new Translation
                                        {
                                            Value = t
                                        });

                                    ecb.AddComponent(entityInQueryIndex, instance, new Target());

                                    if (i == 0)
                                        ecb.AddComponent(entityInQueryIndex, instance, new TeamOne());
                                    else
                                        ecb.AddComponent(entityInQueryIndex, instance, new TeamTwo());

                                    ecb.SetComponent(entityInQueryIndex, instance, new BeeColor { Value = new float4(teamFields[i].TeamColor.r, teamFields[i].TeamColor.g, teamFields[i].TeamColor.b, teamFields[i].TeamColor.a) });
                                    ecb.SetComponent(entityInQueryIndex, instance, new Size() { Value = random.NextFloat(minBeeSize, maxBeeSize) });
                                }
                            }
                        }
                    }

                    for (int j = 0; j < mainField.Length; ++j)
                    {
                        t.x = math.clamp(t.x, mainField[j].Bounds.Min.x, mainField[j].Bounds.Max.x);
                        t.z = math.clamp(t.z, mainField[j].Bounds.Min.z, mainField[j].Bounds.Max.z);
                        t.y = math.max(t.y, mainField[j].Bounds.Floor);

                        translation.Value = t;
                    }
                }

                // Now handle main field collision
                for (int j = 0; j < mainField.Length; ++j)
                {
                    if (t.y <= mainField[j].Bounds.Floor)
                    {
                        v.Value = float3.zero;
                    }
                }
            }).ScheduleParallel(Dependency);

        m_Random = random;

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
