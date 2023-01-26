
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public partial struct BucketEmptySystem : ISystem
{
    EntityQuery m_FilledBuckets;
    EntityQuery m_ReachedWorkers;

    // public class BucketEmptyJob : IJobEntity
    // {
    //     [ReadOnly] public ComponentLookup<WaterAmount> m_BucketWaterAmountLookup;
    //     public EntityCommandBuffer ECB;
    //     GridTemperatures m_GridTemperaturesCopy;
    //
    //     public void Execute(Entity entity, CarriesBucketTag tag, HasReachedDestinationTag reachedDestination)
    //     {
    //         var waterLevel = m_BucketWaterAmountLookup[entity];
    //         waterLevel.currentContain = 0;
    //         ECB.SetComponent(entity, waterLevel);
    //     }
    // }

    public void OnCreate(ref SystemState state)
    {
        m_ReachedWorkers = state.GetEntityQuery(ComponentType.ReadOnly<HasReachedDestinationTag>(), ComponentType.ReadOnly<CarriesBucketTag>());
        // m_OmniWorkerQuery = state.GetEntityQuery(ComponentType.Exclude<TeamInfo>(), ComponentType.ReadWrite<BucketTargetPosition>(), ComponentType.ReadOnly<Position>());
        m_FilledBuckets = state.GetEntityQuery(ComponentType.ReadOnly<BucketTag>(),
            ComponentType.ReadWrite<WaterAmount>(),
            ComponentType.ReadOnly<Position>(),
            ComponentType.ReadOnly<PickedUpTag>());
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // for all carried and filled buckets for workers who has reached their target, set water level on bucket 0
        // get grid cell and radius and decrease fire amount by water level
        var gridRW = SystemAPI.GetSingletonRW<GridTemperatures>();
        var gridRef = gridRW.ValueRW;
        var effectRadius = 4;
        foreach (var workerEntity in m_ReachedWorkers.ToEntityArray(Allocator.Temp))
        {
            var targetOfWorker = SystemAPI.GetComponent<Target>(workerEntity);
            var bucketEntity = SystemAPI.GetComponent<CarriedBucket>(workerEntity).bucket;
            var bucketWaterAmount = SystemAPI.GetComponent<WaterAmount>(bucketEntity);
            if (bucketWaterAmount.currentContain > 0) // we're at a fire, emptying bucket
            {
                var x = targetOfWorker.targetIndex.x;
                var y = targetOfWorker.targetIndex.y;

                var newFireAmount = math.max(gridRef.Get(x, y) - bucketWaterAmount.currentContain, 0f);
                gridRef.Set(x, y, newFireAmount);
                for (int i = -effectRadius + targetOfWorker.targetIndex.x; i < effectRadius + targetOfWorker.targetIndex.x; i++)
                {
                    for (int j = -effectRadius + targetOfWorker.targetIndex.y; j < effectRadius + targetOfWorker.targetIndex.y; j++)
                    {
                        if (i < gridRef.sqrSize && i >= 0 && j >= 0 && j < gridRef.sqrSize)
                        {
                            var newFireAmountNeighbour = math.max(gridRef.Get(i, j) - bucketWaterAmount.currentContain, 0f);
                            gridRef.Set(i, j, newFireAmountNeighbour);
                        }
                    }
                }

                bucketWaterAmount.currentContain = 0;
            }
            else // we're at a water source, filling bucket
            {
                var waterCellEntity = targetOfWorker.targetEntity;
                var waterCellAmount = SystemAPI.GetComponent<WaterAmount>(waterCellEntity);
                var deltaWaterChange = math.max((float)bucketWaterAmount.maxContain, waterCellAmount.currentContain);
                waterCellAmount.currentContain -= (byte)deltaWaterChange;
                bucketWaterAmount.currentContain += (byte)deltaWaterChange;
                if (waterCellAmount.currentContain <= 0)
                {
                    // water cell is depleted, we kill the entity
                    state.EntityManager.DestroyEntity(waterCellEntity);
                }
                else
                {
                    SystemAPI.SetComponent(waterCellEntity, waterCellAmount);
                }
            }

            SystemAPI.SetComponent(bucketEntity, bucketWaterAmount);
        }


    }
}
