using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(FireGridSimulate))]
public class ApplySplash : SystemBase
{
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<BucketBrigadeConfig>();
        RequireForUpdate(GetEntityQuery(new EntityQueryDesc {All = new[] {ComponentType.ReadOnly<Splash>()}}));

        m_EndSimulationEcbSystem = World
           .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var config = GetSingleton<BucketBrigadeConfig>();

        var splashes = new NativeArray<int2>(config.NumberOfChains, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        int numSplashes = 0;

        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

        Entities.ForEach((in Entity entity, in Translation translation, in Splash splash) =>
        {
            splashes[numSplashes++] = ((int2)(translation.Value / config.CellSize).xz);
            ecb.DestroyEntity(entity);
        }).Run();

        Entities.ForEach((ref DynamicBuffer<FireGridCell> cells) =>
        {
            // do things with list
            for (int i = 0; i < numSplashes; ++i)
            {
                int2 pos = splashes[i];
                for(int y= -config.SplashRadius; y<= config.SplashRadius; y++ )
                {
                    int gridY = pos.y + y;
                    if(gridY >= 0 && gridY < config.GridDimensions.y)
                    {
                        for (int x = -config.SplashRadius; x <= config.SplashRadius; x++)
                        {
                            int gridX = pos.x + x;
                            if (gridX >= 0 && gridX < config.GridDimensions.x)
                            {
                                int Index = gridY * config.GridDimensions.x + gridX;
                                FireGridCell cell = cells[Index];
                                float temperatureDrop = config.CoolingStrength;
                                if (!(y == 0 && x == 0))
                                {
                                    float dowseCellStrength = 1f / (math.abs(y * config.CoolingFallOff) + math.abs(x * config.CoolingFallOff));
                                    temperatureDrop = config.CoolingStrength * dowseCellStrength * config.BucketCapacity;
                                }
                                cell.Temperature = math.max(-1.0f, cell.Temperature - temperatureDrop);
                                cells[Index] = cell;
                            }
                        }
                    }
                }
            }

        }).WithDeallocateOnJobCompletion(splashes).ScheduleParallel();


        // Make sure that the ECB system knows about our job
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);

    }
}
