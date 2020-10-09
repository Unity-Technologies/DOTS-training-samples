using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(DeduplicationSystem))]
public class PlantSaplingSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var gameState = GetSingletonEntity<GameState>();
        var prefab = GetComponent<GameState>(gameState).PlantPrefab;

        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        Entities
            .WithAll<Farmer>()
            .WithAll<PlantSaplingTask>()
            .ForEach((
                Entity entity
                , int entityInQueryIndex
                , in TargetEntity target
                , in Position position
            ) =>
            {
                float dist = math.distance(position.Value, target.targetPosition);
                if (dist <= 0.01f)
                {
                    // Instantiate sapling prefab
                    var saplingEntity = ecb.Instantiate(entityInQueryIndex, prefab);
                    ecb.AddComponent(entityInQueryIndex, saplingEntity, new Sappling
                    {
                        age = 0.0f,
                        tileEntity = target.target
                    });
                    ecb.AddComponent<ECSMaterialOverride>(entityInQueryIndex, saplingEntity);
                    ecb.SetComponent(entityInQueryIndex, saplingEntity, new Translation { Value = GetComponent<Translation>(target.target).Value });
                    ecb.AddComponent<NonUniformScale>(entityInQueryIndex, saplingEntity);
                    // Add a sapling reference to the tile
                    ecb.AddComponent(entityInQueryIndex, target.target, new SaplingReference { sapling = saplingEntity });
                    // Reduce the fertility of the tilled component
                    Tilled tilled = GetComponent<Tilled>(target.target);
                    tilled.FertilityLeft--;

                    if (tilled.FertilityLeft > 0)
                    {
                        // Update the fertility and color of the tilled display entity accordingly
                        ecb.SetComponent(entityInQueryIndex, target.target, tilled);
                        const int MAX_FERTILITY = 10;
                        float4 color = math.lerp(new float4(1, 1, 1, 1), new float4(0.3f, 1, 0.3f, 1), (float)tilled.FertilityLeft / MAX_FERTILITY);
                        ecb.SetComponent(entityInQueryIndex, tilled.TilledDisplayPrefab, new ECSMaterialOverride { Value = color });
                    }
                    else
                    {
                        // Make the tile un-tilled
                        ecb.DestroyEntity(entityInQueryIndex, tilled.TilledDisplayPrefab);
                        ecb.RemoveComponent<Tilled>(entityInQueryIndex, target.target);
                    }

                    ecb.RemoveComponent<Assigned>(entityInQueryIndex, target.target);
                    ecb.RemoveComponent<PlantSaplingTask>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<TargetEntity>(entityInQueryIndex, entity);
                }

            }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);

    }
}
