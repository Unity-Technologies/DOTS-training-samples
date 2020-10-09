using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[UpdateAfter(typeof(FireSimulationPropagationSystem))]
public class ThrowChainBucketOnFireSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();
        FireSimulation fireSim = GetSingleton<FireSimulation>();
        
        NativeHashMap<int, Entity> fireCellsByIndex = new NativeHashMap<int, Entity>(1000, Allocator.TempJob);
        NativeHashMap<int, float> fireTemperaturesByIndex = new NativeHashMap<int, float>(1000, Allocator.TempJob);

        Entities
            .WithName("ParseFireCellTemperatures")
            .ForEach((Entity entity, in CellIndex index, in Temperature temperature) =>
            {
                fireCellsByIndex[index.Value] = entity;
                fireTemperaturesByIndex[index.Value] = temperature.Value;
            }).Schedule();
        
        Entities
            .WithName("ThrowChainBicket")
            .WithReadOnly(fireCellsByIndex)
            .WithReadOnly(fireTemperaturesByIndex)
            .WithDisposeOnCompletion(fireCellsByIndex)
            .WithDisposeOnCompletion(fireTemperaturesByIndex)
            .ForEach((Entity bucketEntity, int entityInQueryIndex,
                ref Volume volume,
                in ThrowTag throwTag, in Pos pos) =>
            {
                int centerRow = (int)(pos.Value.x / fireSim.cellSize);
                int centerCol = (int)(pos.Value.y / fireSim.cellSize);
                float tempToReduce = volume.Value * fireSim.extinguishPerWaterVolume;

                int splashRadiusInCells = (int)(fireSim.extinguishRadius / fireSim.cellSize);
                for (int i = 0; i <= splashRadiusInCells; i++)
                {
                    for (int x = -i; x <= i; x++)
                        for (int y = -i; y <= i; y++)
                        {
                            int cellIndex = FireUtils.GridToArrayCoord(centerRow + x, centerCol + y, fireSim.rows);
                            if (cellIndex < 0)
                                continue;
                            if (!fireCellsByIndex.TryGetValue(cellIndex, out Entity cellEntity))
                                continue;
                            float temp = fireTemperaturesByIndex[cellIndex];
                            
                            if (temp > tempToReduce)
                                ecb.SetComponent(entityInQueryIndex, cellEntity, new Temperature { Value = temp - tempToReduce });
                            else
                                ecb.SetComponent(entityInQueryIndex, cellEntity, new Temperature { Value = 0f });
                            tempToReduce -= temp;
                            
                            if (tempToReduce <= 0f)
                                return;
                        }
                }
                
                ecb.SetComponent(entityInQueryIndex, bucketEntity, new Volume {Value = 0f});
                ecb.RemoveComponent<ThrowTag>(entityInQueryIndex, bucketEntity);
            }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}