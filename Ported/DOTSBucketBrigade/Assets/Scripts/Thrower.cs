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
        private EntityArchetype m_SplashEntityArchetype;
        private EndSimulationEntityCommandBufferSystem m_Barrier;

        protected override void OnCreate()
        {
            m_SplashEntityArchetype = EntityManager.CreateArchetype(typeof(Splash), typeof(Translation));
            m_Barrier =
                World.DefaultGameObjectInjectionWorld.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var fireGridEntity = GetSingletonEntity<FireGrid>();
            var fireGridBuffer = GetBufferFromEntity<FireGridCell>();
            
            var translationComponent = GetComponentDataFromEntity<LocalToWorld>();
            var config = GetSingleton<BucketBrigadeConfig>();
            var chainComponent = GetComponentDataFromEntity<Chain>();
            var targetBucketComponent = GetComponentDataFromEntity<TargetBucket>();
            var waterLevelComponent = GetComponentDataFromEntity<WaterLevel>();

            var ecb = m_Barrier.CreateCommandBuffer().ToConcurrent();

            var splashArchetype = m_SplashEntityArchetype;

            Entities.ForEach((Entity entity, int entityInQueryIndex, ref ThrowerState state, ref TargetPosition targetPosition, ref TargetFire targetFire, in NextInChain nextInChain, in Translation position, in Agent agent)
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
                            targetFire.FirePosition = nearestFirePosition;
                            
                            var direction = math.normalize(nearestFirePosition - myChain.ChainStartPosition);
                            var fireFightPoint = nearestFirePosition - (direction * config.HeatRadius);
                            
                            // Set the end of the chain.
                            myChain.ChainEndPosition = fireFightPoint;
                            chainComponent[agent.MyChain] = myChain;
                            
                            // Set the agents target position
                            targetPosition.Target = fireFightPoint;
                            state.State = EThrowerState.WaitForBucket;
                        }
                        break;
                    
                    case EThrowerState.WaitForBucket:
                        if (targetBucket.Target != Entity.Null)
                        {   
                            targetPosition.Target = targetFire.FirePosition;
                            state.State = EThrowerState.WaitUntilInFireRange;
                        }
                        break;
                    
                    case EThrowerState.WaitUntilInFireRange:
                        var fireDistSq = math.distancesq(targetFire.FirePosition.xz, position.Value.xz);

                        if (fireDistSq < config.MovementTargetReachedThreshold)
                        {
                            state.State = EThrowerState.EmptyBucket;
                        }
                        break;
                    
                    case EThrowerState.EmptyBucket:
                        var bucketWaterLevel = waterLevelComponent[targetBucket.Target];
                        bucketWaterLevel.Level = 0;
                        waterLevelComponent[targetBucket.Target] = bucketWaterLevel;

                        var splashEntity = ecb.CreateEntity(entityInQueryIndex, splashArchetype);
                        ecb.SetComponent(entityInQueryIndex, splashEntity, position);
                        
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
            })
                .WithoutBurst()
                .WithReadOnly(fireGridBuffer)
                .WithReadOnly(translationComponent)
                .WithNativeDisableParallelForRestriction(waterLevelComponent)
                .WithNativeDisableParallelForRestriction(targetBucketComponent)
                .WithNativeDisableParallelForRestriction(chainComponent)
                .ScheduleParallel();
            
            m_Barrier.AddJobHandleForProducer(Dependency);
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
                    var fireGridIndex = math.int2(cellColumnIndex, cellRowIndex);
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