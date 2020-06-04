using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace DefaultNamespace
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(Movement))]
    public class Thrower : SystemBase
    {
        protected override void OnCreate()
        {
        }

        protected override void OnUpdate()
        {
            var fireGridEntity = GetSingletonEntity<FireGrid>();
            var fireGridBuffer = GetBufferFromEntity<FireGridCell>();
            
            var translationComponent = GetComponentDataFromEntity<LocalToWorld>();
            var config = GetSingleton<BucketBrigadeConfig>();
            var chainComponent = GetComponentDataFromEntity<Chain>();
            var targetBucketComponent = GetComponentDataFromEntity<TargetBucket>();

            Entities.ForEach((Entity entity, ref ThrowerState state, ref TargetPosition targetPosition, ref TargetFire targetFire, in NextInChain nextInChain, in Translation position, in Agent agent)
                =>
            {
                
                var myChain = chainComponent[agent.MyChain];
                var targetBucket = targetBucketComponent[entity];
                switch (state.State)
                {
                    case EThrowerState.FindFire:
                        var fireGrid = fireGridBuffer[fireGridEntity];
                        if (TryFindNearestCellOnFire(config, fireGrid, position, out var nearestFirePosition, out var nearestFireCell))
                        {
                            // Set the target cell.
                            targetFire.GridIndex = nearestFireCell;
                            
                            // Set the end of the chain.
                            myChain.ChainEndPosition = nearestFirePosition;
                            chainComponent[agent.MyChain] = myChain;
                            
                            // Set the agents target position
                            targetPosition.Target = nearestFirePosition;
                            state.State = EThrowerState.WaitForBucket;
                        }
                        break;
                    
                    case EThrowerState.WaitForBucket:
                        if (targetBucket.Target != Entity.Null)
                        {
                            state.State = EThrowerState.EmptyBucket;
                        }
                        break;
                    
                    case EThrowerState.EmptyBucket:
                        state.State = EThrowerState.WaitUntilChainEndInRangeAndNotCarrying;
                        break;
                    
                    case EThrowerState.WaitUntilChainEndInRangeAndNotCarrying:
                        targetPosition.Target = translationComponent[nextInChain.Next].Position;
                        var nextInChainTargetBucketHasBucket = targetBucketComponent[nextInChain.Next];
                        if (nextInChainTargetBucketHasBucket.Target == Entity.Null)
                        {
                            var chainDistSq = math.distancesq(targetPosition.Target.xz, position.Value.xz);

                            if (chainDistSq < config.MovementTargetReachedThreshold)
                            {
                                state.State = EThrowerState.PassBucket;
                            }
                        }

                        break;
                    
                    case EThrowerState.PassBucket:
                        var nextInChainTargetBucket = targetBucketComponent[nextInChain.Next];
                        nextInChainTargetBucket.Target = targetBucket.Target;
                        targetBucketComponent[nextInChain.Next] = nextInChainTargetBucket;
                        
                        targetBucket.Target = Entity.Null;
                        targetBucketComponent[entity] = targetBucket;
                        
                        state.State = EThrowerState.FindFire;
                        break;
                }
            }).WithReadOnly(fireGridBuffer).WithReadOnly(translationComponent).WithNativeDisableParallelForRestriction(targetBucketComponent).WithNativeDisableParallelForRestriction(chainComponent).ScheduleParallel();
        }

        private static bool TryFindNearestCellOnFire(BucketBrigadeConfig config, in DynamicBuffer<FireGridCell> fireGrid, Translation agentPosition, out float3 closestPosition, out int2 closestGridPosition)
        {
            float closestPositionSq = float.MaxValue;
            closestPosition = default;
            closestGridPosition = default;
            bool found = false;
            for (int i = 0; i < fireGrid.Length; ++i)
            {
                var cell = fireGrid[i];
                if (cell.Temperature > config.Flashpoint)
                {
                    int cellRowIndex = i / config.GridDimensions.x;
                    int cellColumnIndex = i % config.GridDimensions.x;
                    var fireGridIndex = math.int2(cellRowIndex, cellColumnIndex);
                    var fireGridPosition = FireGridInit.CellToWorldSpace(fireGridIndex, config);
                    var distanceSq = math.distancesq(fireGridPosition, agentPosition.Value);
                    if (distanceSq < closestPositionSq)
                    {
                        closestPosition = fireGridPosition;
                        closestGridPosition = fireGridIndex;
                        found = true;
                    }
                }
            }
            return found;
        }
    }
}