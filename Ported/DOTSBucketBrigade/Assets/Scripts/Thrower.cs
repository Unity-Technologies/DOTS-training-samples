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
            
            var config = GetSingleton<BucketBrigadeConfig>();
            var chainComponent = GetComponentDataFromEntity<Chain>();
            var targetBucketComponent = GetComponentDataFromEntity<TargetBucket>();

            // TODO: we are not sure this is required.
            //Dependency = JobHandle.CombineDependencies(Dependency, World.DefaultGameObjectInjectionWorld.GetExistingSystem<Scooper>().MyLastDependency);
            
            Entities.ForEach((ref ThrowerState state, ref TargetPosition targetPosition, ref TargetFire targetFire, ref TargetBucket targetBucket, in NextInChain nextInChain, in Translation position, in Agent agent)
                =>
            {
                
                var myChain = chainComponent[agent.MyChain];
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
                        state.State = EThrowerState.StartWalkingToChainEnd;
                        break;
                    
                    case EThrowerState.StartWalkingToChainEnd:
                        targetPosition.Target = myChain.ChainStartPosition;
                        state.State = EThrowerState.WaitUntilChainEndInRange;
                        break;
                    
                    case EThrowerState.WaitUntilChainEndInRange:
                        var chainDistSq = math.distancesq(myChain.ChainStartPosition.xz, position.Value.xz);

                        if (chainDistSq < config.MovementTargetReachedThreshold)
                        {
                            state.State = EThrowerState.PassBucket;
                        }
                        break;
                    
                    case EThrowerState.PassBucket:
                        var nextInChainTargetBucket = targetBucketComponent[nextInChain.Next];
                        nextInChainTargetBucket.Target = targetBucket.Target;
                        targetBucketComponent[nextInChain.Next] = nextInChainTargetBucket;
                        
                        targetBucket.Target = Entity.Null;
                        
                        state.State = EThrowerState.FindFire;
                        break;
                }
            }).WithReadOnly(fireGridBuffer).WithNativeDisableParallelForRestriction(targetBucketComponent).WithNativeDisableParallelForRestriction(chainComponent).ScheduleParallel();
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