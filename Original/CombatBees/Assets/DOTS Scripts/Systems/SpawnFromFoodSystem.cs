    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Rendering;
    using Unity.Transforms;

    public class SpawnFromFoodSystem : SystemBase
    {
        EntityQuery m_Query;
        EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    
        protected override void OnCreate()
        {
            base.OnCreate();
            // Find the ECB system once and store it for later usage
            m_EndSimulationEcbSystem = World
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            RequireSingletonForUpdate<SpawnZones>();
            RequireForUpdate(m_Query);
        }

        protected override void OnUpdate()
        {
            var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();
            var random = new Random(5678);
            var zones = GetSingleton<SpawnZones>();
            Entities
                .WithName("SpawnFromFood")
                .WithAll<FoodTag>()
                .WithNone<CarrierBee>()
                .WithStoreEntityQueryInField(ref m_Query)
                .ForEach((Entity e, int entityInQueryIndex, ref Translation t) =>
                {
                    if (t.Value.y <= 0)
                    {
                        if (zones.Team1Zone.Contains(t.Value))
                        {
                            for (int i = 0; i < zones.BeesPerFood; ++i)
                            {
                                var newBee = ecb.Instantiate(entityInQueryIndex, zones.BeePrefab);
                                ecb.SetComponent(entityInQueryIndex, newBee, new Translation
                                {
                                    Value = random.NextFloat3(zones.Team1Zone.Min, zones.Team1Zone.Max),
                                });
                                ecb.AddComponent<Team1>(entityInQueryIndex, newBee);
                                ecb.AddComponent(entityInQueryIndex, newBee, new URPMaterialPropertyBaseColor
                                {
                                    Value = new float4(1, 1, 0, 1),
                                });
                            }
                            
                            ecb.DestroyEntity(entityInQueryIndex, e);
                        } 
                        if (zones.Team2Zone.Contains(t.Value))
                        {
                            for (int i = 0; i < zones.BeesPerFood; ++i)
                            {
                                var newBee = ecb.Instantiate(entityInQueryIndex, zones.BeePrefab);
                                ecb.SetComponent(entityInQueryIndex, newBee, new Translation
                                {
                                    Value = random.NextFloat3(zones.Team2Zone.Min, zones.Team2Zone.Max),
                                });
                                ecb.AddComponent<Team1>(entityInQueryIndex, newBee);
                                ecb.AddComponent(entityInQueryIndex, newBee, new URPMaterialPropertyBaseColor
                                {
                                    Value = new float4(0, 1, 1, 1),
                                });
                            }

                            ecb.DestroyEntity(entityInQueryIndex, e);
                        }
                    }

                }).ScheduleParallel();
            m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
        }
    }