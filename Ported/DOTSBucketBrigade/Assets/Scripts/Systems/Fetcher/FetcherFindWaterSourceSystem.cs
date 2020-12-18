using Unity.Collections;
using Unity.Entities;

public class FetcherFindWaterSourceSystem : SystemBase
{
    private EntityQuery _waterSourceQuery;
    private EntityCommandBufferSystem _ecbSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        _waterSourceQuery = GetEntityQuery(typeof(WaterSourceVolume), typeof(Position));
        _ecbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = _ecbSystem.CreateCommandBuffer();

        //Find all valid water volumes
        var waterSourceEntities = _waterSourceQuery.ToEntityArray(Allocator.TempJob);
        var waterSourceVolumes = _waterSourceQuery.ToComponentDataArray<WaterSourceVolume>(Allocator.TempJob);
        var waterSourcePositions = _waterSourceQuery.ToComponentDataArray<Position>(Allocator.TempJob);

        var elapsedTime = Time.ElapsedTime;

        Entities
            .WithAll<Fetcher, FetcherFindWaterSource>()
            .WithDisposeOnCompletion(waterSourceEntities)
            .WithDisposeOnCompletion(waterSourceVolumes)
            .WithDisposeOnCompletion(waterSourcePositions)
            .ForEach((Entity entity, in Position position) =>
        {
            //Find the closest water source with water remaining
            float GetDistanceSquared(Position pos1, Position pos2)
            {
                return (pos1.coord.x - pos2.coord.x) * (pos1.coord.x - pos2.coord.x) +
                       (pos1.coord.y - pos2.coord.y) * (pos1.coord.y - pos2.coord.y);
            }

            var minDistance = float.MaxValue;
            var minDistanceIndex = -1;
            for (var i = 0; i < waterSourceEntities.Length; ++i)
            {
                if (waterSourceVolumes[i].Value <= 0)
                    continue;

                var distance = GetDistanceSquared(position, waterSourcePositions[i]);
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
            ecb.AddComponent(entity, new MovingBot
            {
                StartPosition = position.coord,
                TargetPosition = waterSourcePositions[minDistanceIndex].coord,
                StartTime = elapsedTime,
                TagComponentToAddOnArrival = ComponentType.ReadWrite<FetcherFillingBucket>()
            });
        }).Schedule();

        _ecbSystem.AddJobHandleForProducer(Dependency);
    }
}
