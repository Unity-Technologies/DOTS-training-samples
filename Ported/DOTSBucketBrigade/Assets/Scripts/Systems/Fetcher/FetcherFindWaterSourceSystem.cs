using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class FetcherFindWaterSourceSystem : SystemBase
{
    private EntityQuery _waterSourceQuery;
    private EntityCommandBufferSystem _ecbSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        _waterSourceQuery = GetEntityQuery(typeof(WaterSourceVolume), typeof(Translation));
        _ecbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = _ecbSystem.CreateCommandBuffer();

        //Find all valid water volumes
        var waterSourceEntities = _waterSourceQuery.ToEntityArray(Allocator.TempJob);
        var waterSourceVolumes = _waterSourceQuery.ToComponentDataArray<WaterSourceVolume>(Allocator.TempJob);
        var waterSourceTranslations = _waterSourceQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

        var elapsedTime = Time.ElapsedTime;

        Entities
            .WithAll<Fetcher, FetcherFindWaterSource>()
            .WithDisposeOnCompletion(waterSourceEntities)
            .WithDisposeOnCompletion(waterSourceVolumes)
            .WithDisposeOnCompletion(waterSourceTranslations)
            .ForEach((Entity entity, int entityInQueryIndex, in Translation translation, in AssignedBucket assignedBucket) =>
        {
            //Find the closest water source with water remaining
            float GetDistanceSquared(Translation pos1, Translation pos2)
            {
                return (pos1.Value.x - pos2.Value.x) * (pos1.Value.x - pos2.Value.x) +
                       (pos1.Value.y - pos2.Value.y) * (pos1.Value.y - pos2.Value.y) +
                       (pos1.Value.z - pos2.Value.z) * (pos1.Value.z - pos2.Value.z);
            }

            var minDistance = float.MaxValue;
            var minDistanceIndex = -1;
            for (var i = 0; i < waterSourceEntities.Length; ++i)
            {
                if (waterSourceVolumes[i].Value <= 0)
                    continue;

                var distance = GetDistanceSquared(translation, waterSourceTranslations[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minDistanceIndex = i;
                }
            }

            if (minDistanceIndex == -1)
            {
                //There is no water available, just stand there I guess
                return;
            }

            //Start moving the bot towards the water source
            ecb.RemoveComponent<FetcherFindWaterSource>(entity);
            var movingBot = new MovingBot
            {
                StartPosition = translation.Value,
                TargetPosition = waterSourceTranslations[minDistanceIndex].Value,
                StartTime = elapsedTime,
                TagComponentToAddOnArrival = ComponentType.ReadWrite<FetcherFillingBucket>()
            };
            ecb.AddComponent(entity, movingBot);
        }).Schedule();

        _ecbSystem.AddJobHandleForProducer(Dependency);
    }
}
